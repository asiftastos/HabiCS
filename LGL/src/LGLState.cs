using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;

namespace LGL
{
    public static class LGLState
    {
        public static void InitState(GameWindow game)
        {
            DepthTest(true);
            CullFace(true);
            CullFaceBack();
            Blend(false);
            Viewport(0, 0, game.ClientSize.X, game.ClientSize.Y);
        }

        public static void BeginDraw()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public static void EndDraw(GameWindow window)
        {
            window.SwapBuffers();
        }

        public static void Viewport(int x, int y, int width, int height)
        {
            GL.Viewport(x, y, width, height);
        }

        /*
         * Depth Test
         */
        public static void DepthTest(bool enable)
        {
            if (enable)
            {
                GL.Enable(EnableCap.DepthTest);
            }else
            {
                GL.Disable(EnableCap.DepthTest);
            }
        }

        /*
         * CullFace
         */
        public static void CullFace(bool enable)
        {
            if(enable)
            {
                GL.Enable(EnableCap.CullFace);
            }else
            {
                GL.Disable(EnableCap.CullFace);
            }
        }

        public static void CullFaceFront()
        {
            GL.CullFace(CullFaceMode.Front);
        }

        public static void CullFaceBack() 
        { 
            GL.CullFace (CullFaceMode.Back); 
        }

        public static void CullFaceBoth()
        {
            GL.CullFace(CullFaceMode.FrontAndBack);
        }

        /*
         * Blending
         */
        public static void Blend(bool enable)
        {
            if( enable)
            {
                GL.Enable(EnableCap.Blend);
            }else
            {
                GL.Disable(EnableCap.Blend);
            }
        }

        /*
         * Polygon Mode
         * 
         * NOTE: For now only FrontAndBack is set
         */
        public static void PolygonModeFill()
        {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        public static void PolygonModeLine()
        {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        }

        public static void PolygonModePoint()
        {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Point);
        }

        /*
         * Front face winding
         */
        public static void FrontFaceClockWise()
        {
            GL.FrontFace(FrontFaceDirection.Cw);
        }

        public static void FrontFaceCounterClockWise()
        {
            GL.FrontFace(FrontFaceDirection.Ccw);
        }
    }
}
