using System;
using System.Collections.Generic;
using HabiCS.Loaders;
using HabiCS.Graphics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace HabiCS.UI
{
    public class Label: IUIElem
    {
        private int ebo;
        private int indicesToDraw;
        private UIMesh _mesh;
        private UIText _text;
        private UIRect _bounds;

        private Font _font;

        public bool Inderactable {get; set;}

        public Label(float x, float y, float w, float h, string text, Font font)
        {
            _bounds = new UIRect((int)x, (int)y, (int)w, (int)h);
            _text = new UIText(text);
            _mesh = new UIMesh();
            _font = font;
            ebo = GL.GenBuffer();
            Inderactable = false;
        }

        public void Draw(ref Shader sh)
        {
            if(_text.Updated)
                UpdateText();

            //GL.UniformMatrix4(Font.ModelLoc, false, ref model);
            sh.UploadColor("color", Color4.Black);
            sh.UploadBool("text", true);
            _mesh.DrawIndexed();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.DrawElements(BeginMode.Triangles, indicesToDraw, DrawElementsType.UnsignedShort, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        private void UpdateText()
        {
            List<TextureVertex> verts = new List<TextureVertex>();
            List<ushort> indices = new List<ushort>();
            float xpos = _bounds.Position.X;
            foreach (var item in _text.Text)
            {
                if (!_font.Characters.ContainsKey(item))
                    continue;
                
                Font.Glyph glyph = _font.Characters[item];
                if(item == ' ') {
                    xpos += glyph.Width;
                    continue;
                }
                

                // Find the texture coordinates for this glyph
                float u1 = (float)glyph.X / _font.Width;
                float u2 = (float)(glyph.X + glyph.Width) / _font.Width;
                float v1 = (float)glyph.Y / _font.Height;
                float v2 = (float)(glyph.Y + glyph.Height) / _font.Height;

                // add 4 vertices for each corner to draw the glyph as a texture
                // Use of indices below to tell the triangles
                // NOTE  Try to add the last 2 for each glyph and use the last 2 of the previous as the start for the next
                verts.Add(new TextureVertex(xpos, _bounds.Position.Y, -1.0f, u1, v2));
                verts.Add(new TextureVertex(xpos, _bounds.Position.Y + glyph.Height, -1.0f, u1, v1));
                verts.Add(new TextureVertex(xpos + glyph.Width, _bounds.Position.Y, -1.0f, u2, v2));
                verts.Add(new TextureVertex(xpos + glyph.Width, _bounds.Position.Y + glyph.Height, -1.0f, u2, v1));

                // Advance to the next position a glyph can be drawn
                xpos += glyph.Advance;

                // Indices for the above vertices to create the 2 triangles for the quad
                int last = verts.Count - 1;
                indices.AddRange(new ushort[] { (ushort)(last - 3), (ushort)(last - 1), (ushort)(last - 2) });
                indices.AddRange(new ushort[] { (ushort)(last - 2), (ushort)(last - 1), (ushort)(last) });
            }

            indicesToDraw = indices.Count;

            _mesh.BuildText(verts.ToArray(), new UIMesh.Attribute[]{
                new UIMesh.Attribute(0,3,TextureVertex.SizeInBytes, 0),
                new UIMesh.Attribute(1,2, TextureVertex.SizeInBytes, Vector3.SizeInBytes)
            });

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(ushort), indices.ToArray(), BufferUsageHint.StaticDraw);
            
            _text.Updated = false;
        }

        public void ProcessMouseDown(MouseButtonEventArgs e)
        {
        }

        #region DISPOSABLE PATTERN
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _mesh.Dispose();
                    GL.DeleteBuffer(ebo);
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}