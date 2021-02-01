using System;
using OpenTK.Windowing.Common;
using HabiCS.Loaders;

namespace HabiCS.UI
{
    public interface IUIElem: IDisposable
    {
        public bool Inderactable {get; set;}

        void ProcessMouseDown(MouseButtonEventArgs e);

        void Draw(ref Shader sh);
    }
}