using System;
using System.Collections.Generic;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using LGL.Loaders;
using LGL.Gfx;
using Leopotam.EcsLite;
using HabiCS.UI.Systems;
using HabiCS.UI.Components;

namespace HabiCS.UI
{
    public class UIScreen: IDisposable
    {
        private Game game;
        private FontRenderer fontRenderer;
        private SceneManager sceneManager;

        private EcsWorld world;
        private EcsSystems systems;

        public FontRenderer FontRenderer { get { return fontRenderer; } }

        public UIScreen(Game g)
        {
            game = g;
            sceneManager = g.SceneManager;
            fontRenderer = new FontRenderer(g.ClientSize.X, g.ClientSize.Y);

            world = new EcsWorld();
            systems = new EcsSystems(world);
        }

        public virtual void Load()
        {
            systems
                .Add(new UITextSystem(fontRenderer))
                .Init();
        }

        public virtual void Render(double time)
        {
            systems?.Run();
            fontRenderer.EndRender();
        }

        
        public void OnMouseDown(MouseButtonEventArgs e, Vector2 mousePos)
        {
        }

        public int AddLabel(string text, Vector2 position) 
        {
            var entity = world.NewEntity();

            var posPool = world.GetPool<UIPosition>();
            ref UIPosition p = ref posPool.Add(entity);
            p.position = position;

            var textPool = world.GetPool<UIText>();
            ref UIText t = ref textPool.Add(entity);
            t.text = text;

            return entity;
        }

        public void SetLabel(int labelId, string newtext)
        {
            if(world is null)
                return;

            var textPool = world.GetPool<UIText>();
            ref UIText t = ref textPool.Get(labelId);
            t.text = newtext;
        }

#region DISPOSABLE PATTERN

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    systems.Destroy();
                    systems = null;
                    world.Destroy();
                    world = null;

                    fontRenderer.Dispose();
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