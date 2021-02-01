using System;
using HabiCS.Loaders;

namespace HabiCS.UI
{
    public interface IUIElem: IDisposable
    {
         void Draw(ref Shader sh);
    }
}