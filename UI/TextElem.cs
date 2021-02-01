using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using HabiCS.Loaders;
using HabiCS.Graphics;

namespace HabiCS.UI
{
    public class TextElem : IDisposable
    {
        private UIMesh mesh;
        private int ebo;
        private int indicesToDraw;
        
        private Matrix4 model;
        private Vector2 scale;

        public  Matrix4 Model { get { return model; } }
        
        public Vector2 Position {get; set;}

        public Font Font{get; set;}

        public UIText Data { get; set; }
        
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
            ebo = GL.GenBuffer();
            Data = new UIText(text);
            model = Matrix4.Identity;
            mesh = new UIMesh();
        }

        private void UpdateText()
        {
            List<TextVertex> verts = new List<TextVertex>();
            List<ushort> indices = new List<ushort>();
            float xpos = Position.X;
            foreach (var item in Data.Text)
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

            mesh.BuildText(verts.ToArray(), new UIMesh.Attribute[]{
                new UIMesh.Attribute(0,3,TextVertex.SizeInBytes, 0),
                new UIMesh.Attribute(1,2, TextVertex.SizeInBytes, Vector3.SizeInBytes)
            });

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(ushort), indices.ToArray(), BufferUsageHint.StaticDraw);
            
            Data.Updated = false;
        }

        public void Draw()
        {
            if(Data.Updated)
                UpdateText();

            GL.UniformMatrix4(Font.ModelLoc, false, ref model);
            mesh.DrawIndexed();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.DrawElements(BeginMode.Triangles, indicesToDraw, DrawElementsType.UnsignedShort, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }


        #region DISPOSABLE PATTERN
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    mesh.Dispose();
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