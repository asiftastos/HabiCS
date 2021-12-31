/*
 *  [ ] DrawText must take into account ts size we pass as a scale factor
 *      [ ] Should this be seperate for each text we draw
 */
using System;
using OpenTK.Graphics.OpenGL4;
using LGL.Loaders;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace LGL.Gfx
{
    public class FontRenderer : IDisposable
    {
        private Font font;

        private int vao;
        private int vbo;
        private int ebo;

        private Shader sdf;

        private Matrix4 scale;
        private Matrix4 ortho;

        private List<VertexTexture> vertices;
        private List<ushort> indices;

        public FontRenderer(int width, int height)
        {
            vertices = new List<VertexTexture>();
            indices = new List<ushort>();

            font = Font.Load("Assets/Fonts/font.json", 0, 0);

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            sdf = Shader.Load("Text", 2, "Assets/Shaders/text.vert", "Assets/Shaders/text.frag");
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
            float xpos = position.X;
            float scaleFactor = size / font.Size;
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
                // NOTE  Try to add the last 2 for each glyph and use the last 2 of the previous as the start for the next
                vertices.Add(new VertexTexture(xpos, ypos, -1.0f, u1, v2));
                vertices.Add(new VertexTexture(xpos, ypos + glyph.Height, -1.0f, u1, v1));
                vertices.Add(new VertexTexture(xpos + glyph.Width, ypos, -1.0f, u2, v2));
                vertices.Add(new VertexTexture(xpos + glyph.Width, ypos + glyph.Height, -1.0f, u2, v1));

                //Advance to the next position a glyph can be drawn, add padding(defaults to 1.0f)
                xpos += glyph.Advance;

                // Indices for the above vertices to create the 2 triangles for the quad
                int last = vertices.Count - 1;
                indices.AddRange(new ushort[] { (ushort)(last - 3), (ushort)(last - 1), (ushort)(last - 2) });
                indices.AddRange(new ushort[] { (ushort)(last - 2), (ushort)(last - 1), (ushort)(last) });
            }
        }

        public void EndRender()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * VertexTexture.SizeInBytes, vertices.ToArray(), BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexTexture.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, VertexTexture.SizeInBytes, Vector3.SizeInBytes);
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(ushort), indices.ToArray(), BufferUsageHint.DynamicDraw);

            font.Bind();

            sdf.Use();
            sdf.UploadMatrix("ortho", ref ortho);
            sdf.UploadMatrix("model", ref scale);
            sdf.UploadColor("color", Color4.White);
            GL.DrawElements(BeginMode.Triangles, indices.Count, DrawElementsType.UnsignedShort, 0);

            font.Unbind();

            GL.BindVertexArray(0);

            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);

            vertices.Clear();
            indices.Clear();
        }
    }
}
