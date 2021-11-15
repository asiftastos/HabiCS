using System;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using HabiCS.Loaders;

namespace HabiCS.UI
{
    public interface IUIElem: IDisposable
    {
        public bool Inderactable {get; set;}

        bool ProcessMouseDown(MouseButtonEventArgs e, Vector2 mousePos);

        void Draw(ref Shader sh);
    }
}