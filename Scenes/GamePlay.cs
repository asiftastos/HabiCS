using System.Collections.Generic;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using HabiCS.UI;
using HabiCS.Loaders;

namespace HabiCS.Scenes
{
    public class GamePlay: Scene
    {
        private const int GRIDWIDTH = 256;
        private const int GRIDHEIGHT = 256;
        private const int CELLSIZE = 2;
        private Vector2 gridMin;
        private Vector2 gridMax;

        private int gridVAO;
        private int gridVBO;
        private Color4 gridColor;
        private Shader gridShader;
        private Shader instancedShader;
        private int instancedVAO;
        private int instancedVertexVBO;
        private int instancedColorVBO;
        private int instancedOffsetVBO;

        private int colorStructSize;

        private Matrix4 _ortho;

        private List<Color4> elementColorsBuffer;
        private List<Vector2> elementOffsetBuffer;


        public GamePlay(Game g):base("GamePlay", g)
        {
            elementColorsBuffer = new List<Color4>();
            elementOffsetBuffer = new List<Vector2>();
        }

        public override void ProcessKeyInput(KeyboardKeyEventArgs e)
        {
            base.ProcessKeyInput(e);
            if(e.Key == Keys.D1)
            {
                var scene = new Start(game);
                game.SceneManager.ChangeScene(scene);
                Label l = (Label)game.SceneManager.CurrentScreen.GetElem("Name");
                if(l is not null)
                    l.Text = scene.Name;
            }
        }

        public override void Load()
        {
            base.Load();
            GL.ClearColor(Color4.Black);

            SetupGrid();

            SetupInstancedResources();
        }

        public override void Update(double time)
        {
            base.Update(time);
        }

        public override void Render(double time)
        {
            base.Render(time);
            if(game.RenderPass == Graphics.RenderPass.PASS3D)
                return;
            
            gridShader.Use();
            gridShader.UploadMatrix("ortho", ref _ortho);
            gridShader.UploadColor("color", gridColor);
            GL.BindVertexArray(gridVAO);
            GL.DrawArrays(PrimitiveType.LineLoop, 0, 4);
            GL.BindVertexArray(0);

            instancedShader.Use();
            instancedShader.UploadMatrix("ortho", ref _ortho);
            GL.BindVertexArray(instancedVAO);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, elementOffsetBuffer.Count);
            GL.BindVertexArray(0);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            elementColorsBuffer.Clear();
            elementOffsetBuffer.Clear();
            
            gridShader.Dispose();
            instancedShader.Dispose();
            
            GL.DeleteBuffer(instancedVertexVBO);
            GL.DeleteBuffer(instancedColorVBO);
            GL.DeleteBuffer(instancedOffsetVBO);
            GL.DeleteVertexArray(instancedVAO);

            GL.DeleteBuffer(gridVBO);
            GL.DeleteVertexArray(gridVAO);
        }

        private void SetupGrid()
        {
            gridMin = new Vector2((game.ClientSize.X - GRIDWIDTH) / 2, (game.ClientSize.Y - GRIDHEIGHT) / 2);
            gridMax = new Vector2(gridMin.X + GRIDWIDTH, gridMin.Y + GRIDHEIGHT);
            float[] verts = {
                gridMin.X, gridMin.Y, -1.0f,
                gridMax.X, gridMin.Y, -1.0f,
                gridMax.X, gridMax.Y, -1.0f,
                gridMin.X, gridMax.Y, -1.0f
            };

            gridVAO = GL.GenVertexArray();
            GL.BindVertexArray(gridVAO);
            gridVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, gridVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            gridShader = Shader.Load("Grid", 2, "Assets/Shaders/grid.vert", "Assets/Shaders/grid.frag");
            gridShader.SetupUniforms(new string[]{"ortho", "color"});

            gridColor = Color4.White;
            _ortho = game.SceneManager.Ortho;
        }

        private void SetupInstancedResources()
        {
            instancedShader = Shader.Load("Instanced", 2, "Assets/Shaders/instanced.vert", "Assets/Shaders/instanced.frag");
            instancedShader.SetupUniforms(new string[]{"ortho"});

            float[] verts = {
                0.0f, 0.0f,
                (float)CELLSIZE, 0.0f,
                0.0f, (float)CELLSIZE,
                0.0f, (float)CELLSIZE,
                (float)CELLSIZE, 0.0f,
                (float)CELLSIZE, (float)CELLSIZE,
            };

            instancedVAO = GL.GenVertexArray();
            GL.BindVertexArray(instancedVAO);

            instancedVertexVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, instancedVertexVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, sizeof(float) * 2, 0);
            GL.EnableVertexAttribArray(0);

            unsafe
            {
                colorStructSize = sizeof(Color4);
            }


            for(int i = 0; i < 100; i++)
            {
                elementColorsBuffer.Add(Color4.Yellow);
                elementOffsetBuffer.Add(new Vector2(gridMin.X + i, gridMin.Y + i));
            }
            instancedColorVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, instancedColorVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, elementColorsBuffer.Count * colorStructSize, elementColorsBuffer.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, colorStructSize, 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribDivisor(1, 1);

            instancedOffsetVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, instancedOffsetVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, elementOffsetBuffer.Count * Vector2.SizeInBytes, elementOffsetBuffer.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribDivisor(2, 1);

            GL.BindVertexArray(0);
        }
    }
}