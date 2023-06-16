/*
 *  TODO
 *  [ ] DrawText must take into account ts size we pass as a scale factor
 *      [ ] Should this be seperate for each text we draw
 *  [x] Implement a RenderBatch for all text
 *      [ ] Check if render batch is filled, draw it and start a new one for bigger texts
 */
using LGL.Loaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace LGL.Gfx
{
    public class FontRenderer : IDisposable
    {
        private readonly int maxRenderBatchChars = 512;

        private Font font;

        private int vao;
        private int vbo;
        private int ebo;

        private Shader sdf;

        private Matrix4 scale;
        private Matrix4 ortho;

        private VertexTexture[] vertices;
        private ushort[] indices;
        private int verticesCount = 0;
        private int indicesCount = 0;
        
        public FontRenderer(int width, int height)
        {
            vertices = new VertexTexture[maxRenderBatchChars * 4];
            indices = new ushort[maxRenderBatchChars * 6];

            font = Font.Load("Assets/Fonts/font.json", 0, 0);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, maxRenderBatchChars * VertexTexture.SizeInBytes * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, maxRenderBatchChars * sizeof(ushort) * 6, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexTexture.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, VertexTexture.SizeInBytes, Vector3.SizeInBytes);
            GL.EnableVertexAttribArray(1);


            sdf = Shader.Load("Text", 2, "Assets/Shaders/text.vert", "Assets/Shaders/text.frag", false);
            sdf.SetupUniforms(new string[] { "ortho", "model", "color" });

            scale = Matrix4.CreateScale(0.8f, 0.8f, 1.0f);
            ortho = Matrix4.CreateOrthographicOffCenter(0.0f, (float)width, 0.0f, (float)height, 0.1f, 1.0f);
        }

        public void Dispose()
        {
            font.Dispose();
            sdf.Dispose();
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
            GL.DeleteVertexArray(vao);
        }

        public void DrawText(string text, Vector2 position, float size)
        {
            int vertIndex = 0;
            int indicesIndex = 0;
            float xpos = position.X;
            float scaleFactor = size / font.Size;
            scale = Matrix4.CreateScale(scaleFactor); //this is for all text we draw and counts the last DrawText call
            foreach (var c in text)
            {
                if (!font.Characters.ContainsKey(c))
                    continue;

                Font.Glyph glyph = font.Characters[c];

                // Find the texture coordinates for this glyph
                float u1 = (float)(glyph.X + glyph.OriginX) / font.Width;
                float u2 = (float)(glyph.X + glyph.OriginX + glyph.Width) / font.Width;
                float v1 = (float)glyph.Y / font.Height;
                float v2 = (float)(glyph.Y + glyph.Height) / font.Height;

                // calculate y position for glyphs that are below baseline
                float ypos = position.Y - (glyph.Height - glyph.OriginY);

                // add 4 vertices for each corner to draw the glyph as a texture
                // Use of indices below to tell the triangles
                vertices[vertIndex] = new VertexTexture(xpos, ypos, -1.0f, u1, v2);
                vertices[vertIndex + 1] = new VertexTexture(xpos + glyph.Width, ypos, -1.0f, u2, v2);
                vertices[vertIndex + 2] = new VertexTexture(xpos + glyph.Width, ypos + glyph.Height, -1.0f, u2, v1);
                vertices[vertIndex + 3] = new VertexTexture(xpos, ypos + glyph.Height, -1.0f, u1, v1);
                vertIndex += 4;

                //Advance to the next position a glyph can be drawn, add padding(defaults to 1.0f)
                xpos += glyph.Advance;

                // Indices for the above vertices to create the 2 triangles for the quad
                int last = vertIndex - 1;
                indices[indicesIndex] = (ushort)(last - 3);
                indices[indicesIndex + 1] = (ushort)(last - 2);
                indices[indicesIndex + 2] = (ushort)(last - 1);
                indices[indicesIndex + 3] = (ushort)(last - 3);
                indices[indicesIndex + 4] = (ushort)(last - 1);
                indices[indicesIndex + 5] = (ushort)(last);
                indicesIndex += 6;
            }
            verticesCount = vertIndex;
            indicesCount = indicesIndex;
        }

        public void BeginRender()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public void EndRender()
        {
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferSubData<VertexTexture>(BufferTarget.ArrayBuffer, IntPtr.Zero, verticesCount * VertexTexture.SizeInBytes, vertices);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferSubData<ushort>(BufferTarget.ElementArrayBuffer, IntPtr.Zero, indicesCount * sizeof(ushort), indices);

            RenderBatch();

            font.Unbind();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
        }

        private void RenderBatch()
        {
            font.Bind();

            sdf.Enable();
            sdf.UploadMatrix("ortho", ref ortho);
            sdf.UploadMatrix("model", ref scale);
            sdf.UploadColor("color", Color4.White);
            GL.DrawElements(BeginMode.Triangles, indicesCount, DrawElementsType.UnsignedShort, 0);
        }
    }
}
