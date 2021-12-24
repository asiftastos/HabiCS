using System;
using LGL.Loaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;
using HabiCS.World;

namespace HabiCS.Scenes
{
    class Simple : Scene
    {
        private Shader shader;
        private Matrix4 model;

        private Camera cam;
        private const double camSpeed = 50.0;
        private const double camRotSpeed = 3.5;
        private bool camRotMode;

        private Matrix4 ortho;
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
        private int fps;
        private int fpsCounter;
        private double timeCounter;

        public Simple(Game g) :base("Simple", g)
        {
            camRotMode = false;
            xcenter = (int)game.ClientSize.X / 2;
            ycenter = (int)game.ClientSize.Y / 2;
            map = new Map(2);
            fps = 0;
            fpsCounter = 0;
            timeCounter = 0.0;
        }

        public override void Load()
        {
            base.Load();

            chunkMeshBuilder = new ChunkMeshBuilder();

            chunkGenerator = new ChunkGenerator(1975);

            map.Populate(chunkGenerator, chunkMeshBuilder);
            
            shader = Shader.Load("Simple", 2, "Assets/Shaders/simple.vert", "Assets/Shaders/simple.frag");
            shader.Use();
            vpLoc = GL.GetUniformLocation(shader.ShaderID, "VP");
            mLoc = GL.GetUniformLocation(shader.ShaderID, "M");

            model = Matrix4.Identity;
            vp = Matrix4.Identity;

            ortho = Matrix4.CreateOrthographicOffCenter(0.0f, (float)game.ClientSize.X, 0.0f, (float)game.ClientSize.Y, 0.1f, 1.0f);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), game.ClientSize.X / game.ClientSize.Y, 0.1f, 1000.0f);
            cam = new Camera(new Vector3(0.0f, 70.0f, 200.0f));
            vp = cam.View * projection;

            font = Font.Load("Assets/Fonts/font.json", game.ClientSize.X, game.ClientSize.Y);

            float textWidth = (float)game.SceneManager.MeasureText("TAB = Toggle Camera Rotation, F3 = Toggle draw chunk borders");
            UI.Label infoText = new UI.Label(0.0f, 100.0f, textWidth, 40.0f, "TAB = Toggle Camera Rotation, F3 = Toggle draw chunk borders", font);
            game.SceneManager.CurrentScreen.Elements.Add("DebugInfo", infoText);
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
            
            //2D
            //GL.Disable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
          
            //GL.Disable(EnableCap.Blend);
            //GL.Enable(EnableCap.DepthTest);
        }

        public override void ProcessKeyInput(KeyboardKeyEventArgs e)
        {
            base.ProcessKeyInput(e);

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

            font.Dispose();

            base.Dispose(disposing);
        }
    }
}
