using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using HabiCS.Loaders;

namespace HabiCS.UI
{
    public class TextElem : IDisposable
    {
        private struct TextVertex
        {
            public static int SizeInBytes { get { return Vector3.SizeInBytes + Vector2.SizeInBytes; } }

            public TextVertex(float x, float y, float z, float u, float v)
            {
                position = new Vector3(x, y, z);
                texCoords = new Vector2(u, v);
            }

            public Vector3 position;
            public Vector2 texCoords;
        }

        private int vao;
        private int vbo;
        private int ebo;
        private int indicesToDraw;
        private bool disposedValue;
        private bool textChanged;
        private string _text;

        private Matrix4 model;
        private Vector2 scale;

        public  Matrix4 Model { get { return model; } }
        
        public string Text 
        {
            get => _text; 
            set
            {
                _text = value;
                textChanged = true;
            }
        }

        public Vector2 Position {get; set;}

        public Font Font{get; set;}
        
        public Vector2 Scale 
        { 
            get 
            {
                return scale;
            } 
            set 
            {
                scale = value;
                model = Matrix4.CreateScale(scale.X, scale.Y, 1.0f);
            }
        }
        public TextElem(string text, Vector2 pos)
        {
            Position = pos;
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();
            Text = text;
            textChanged = true;
            vao = GL.GenVertexArray();
            model = Matrix4.Identity;
        }

        private void UpdateText()
        {
            List<TextVertex> verts = new List<TextVertex>();
            List<ushort> indices = new List<ushort>();
            float xpos = Position.X;
            foreach (var item in Text)
            {
                if (!Font.Characters.ContainsKey(item))
                    continue;
                
                Font.Glyph glyph = Font.Characters[item];
                if(item == ' ') {
                    xpos += glyph.Width;
                    continue;
                }
                

                // Find the texture coordinates for this glyph
                float u1 = (float)glyph.X / Font.Width;
                float u2 = (float)(glyph.X + glyph.Width) / Font.Width;
                float v1 = (float)glyph.Y / Font.Height;
                float v2 = (float)(glyph.Y + glyph.Height) / Font.Height;

                // add 4 vertices for each corner to draw the glyph as a texture
                // Use of indices below to tell the triangles
                // NOTE  Try to add the last 2 for each glyph and use the last 2 of the previous as the start for the next
                verts.Add(new TextVertex(xpos, Position.Y, -1.0f, u1, v2));
                verts.Add(new TextVertex(xpos, Position.Y + glyph.Height, -1.0f, u1, v1));
                verts.Add(new TextVertex(xpos + glyph.Width, Position.Y, -1.0f, u2, v2));
                verts.Add(new TextVertex(xpos + glyph.Width, Position.Y + glyph.Height, -1.0f, u2, v1));

                // Advance to the next position a glyph can be drawn
                xpos += glyph.Advance;

                // Indices for the above vertices to create the 2 triangles for the quad
                int last = verts.Count - 1;
                indices.AddRange(new ushort[] { (ushort)(last - 3), (ushort)(last - 1), (ushort)(last - 2) });
                indices.AddRange(new ushort[] { (ushort)(last - 2), (ushort)(last - 1), (ushort)(last) });
            }

            indicesToDraw = indices.Count;

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, TextVertex.SizeInBytes * verts.Count, verts.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, TextVertex.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, TextVertex.SizeInBytes, Vector3.SizeInBytes);
            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(ushort), indices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);

            
            textChanged = false;
        }

        public void Draw()
        {
            if(textChanged)
                UpdateText();

            GL.UniformMatrix4(Font.ModelLoc, false, ref model);
            GL.BindVertexArray(vao);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.DrawElements(BeginMode.Triangles, indicesToDraw, DrawElementsType.UnsignedShort, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }


        #region DISPOSABLE PATTERN

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    GL.DeleteBuffer(vbo);
                    GL.DeleteBuffer(ebo);
                    GL.DeleteVertexArray(vao);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
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