using HabiCS.Loaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;
using HabiCS.World;
using HabiCS.UI;

namespace HabiCS.Scenes
{
    class Simple : Scene
    {
        Game game;
        private Shader shader;
        private Matrix4 model;

        private Camera cam;
        private const double camSpeed = 50.0;
        private const double camRotSpeed = 3.5;
        private bool camRotMode;

        private Matrix4 projection;
        private Matrix4 vp;

        private int vpLoc;
        private int mLoc;

        private int xcenter;
        private int ycenter;

        private Map map;
        private ChunkGenerator chunkGenerator;
        private ChunkMeshBuilder chunkMeshBuilder;

        private Font font;
        private TextElem perfText;
        private TextElem debugText;
        private int fps;
        private int fpsCounter;
        private double timeCounter;

        public Simple(Game g) :base("Simple")
        {
            game = g;
            camRotMode = false;
            xcenter = (int)game.ClientSize.X / 2;
            ycenter = (int)game.ClientSize.Y / 2;
            map = new Map(2);
            fps = 0;
            fpsCounter = 0;
            timeCounter = 0.0;
            shader = new Shader("Simple", 2);
        }

        public override void Load()
        {
            base.Load();

            //float blockSize = 1.0f; //the distance between blocks,basically the block size

            chunkMeshBuilder = new ChunkMeshBuilder();

            chunkGenerator = new ChunkGenerator(1975);

            map.Populate(chunkGenerator, chunkMeshBuilder);
            
            shader.CompileVertexFromFile("Assets/Shaders/simple.vert");
            shader.CompileFragmentFromFile("Assets/Shaders/simple.frag");
            shader.CreateProgram();
            shader.Use();
            vpLoc = GL.GetUniformLocation(shader.ShaderID, "VP");
            mLoc = GL.GetUniformLocation(shader.ShaderID, "M");

            model = Matrix4.Identity;
            vp = Matrix4.Identity;

            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), game.ClientSize.X / game.ClientSize.Y, 0.1f, 1000.0f);
            cam = new Camera(new Vector3(0.0f, 70.0f, 200.0f));
            vp = cam.View * projection;

            font = Font.Load("Assets/Fonts/font.json", game.ClientSize.X, game.ClientSize.Y);
            string debug = $"Game client size: {game.ClientSize.X}, {game.ClientSize.Y}";
            perfText = new TextElem(debug, new Vector2(0.0f, game.ClientSize.Y - font.Size - 4.0f));
            perfText.Font = font;
            debugText = new TextElem("", new Vector2(0.0f, 0.0f));
            debugText.Font = font;
        }

        public override void Update(double time)
        {
            base.Update(time);

            if(camRotMode)
            {
                if(game.MouseState.Position != game.MouseState.PreviousPosition)
                {
                    float xoffset = game.MousePosition.X - xcenter;
                    float yoffset = ycenter - game.MousePosition.Y;
                    cam.Rotate(xoffset * (float)(camRotSpeed * time), yoffset * (float)(camRotSpeed * time));
                    game.MousePosition = new Vector2((float)xcenter, (float)ycenter);
                }
            }

            if (game.IsKeyDown(Keys.W))
                cam.MoveForward((float)(time * camSpeed));
            if (game.IsKeyDown(Keys.S))
                cam.MoveBack((float)(time * camSpeed));
            if (game.IsKeyDown(Keys.D))
                cam.MoveRight((float)(time * camSpeed));
            if (game.IsKeyDown(Keys.A))
                cam.MoveLeft((float)(time * camSpeed));
            if (game.IsKeyDown(Keys.Space))
                cam.MoveUp((float)(time * camSpeed));
            if (game.IsKeyDown(Keys.LeftShift))
                cam.MoveDown((float)(time * camSpeed));

            cam?.Update();
        }

        public override void Render(double time)
        {
            fpsCounter++;
            timeCounter += time;
            if(timeCounter >= 1.0)
            {
                timeCounter = 0.0;
                fps = fpsCounter;
                fpsCounter = 0;
                perfText.Text = $"FPS: {fps}";
            }

            base.Render(time);

            vp = cam.View * projection;

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            
            shader.Use();
            GL.UniformMatrix4(mLoc, false, ref model);
            GL.UniformMatrix4(vpLoc, false, ref vp);
            int totalVerts = 0;
            map.Draw(ref totalVerts);
            
            perfText.Font.Bind();
            perfText.Draw();
            debugText.Text = $"Total scene's vertices: {totalVerts}";
            debugText.Draw();
            perfText.Font.Unbind();
        }

        public override void ProcessInput(KeyboardKeyEventArgs e)
        {
            base.ProcessInput(e);

            if(e.Key == Keys.Tab)
            {
                camRotMode = !camRotMode;
                game.MousePosition = new Vector2((float)xcenter, (float)ycenter);
            }
            if(e.Key == Keys.F3)
            {
                map.ShowDebug = !map.ShowDebug;
            }
        }

        protected override void Dispose(bool disposing)
        {
            // dispose resources
            shader.Dispose();
            map.Dispose();

            debugText.Dispose();
            font.Dispose();

            base.Dispose(disposing);
        }
    }
}
