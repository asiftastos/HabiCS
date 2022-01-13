using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leopotam.EcsLite;
using HabiCS.UI.Components;
using LGL.Gfx;

namespace HabiCS.UI.Systems
{
    public class UITextSystem : IEcsInitSystem, IEcsRunSystem
    {
        //private EcsFilter textFilter;  //entities with UIPosition and UIText components
        private FontRenderer _fontRenderer;

        public UITextSystem(FontRenderer renderer)
        {
            _fontRenderer = renderer;
        }

        public void Init(EcsSystems systems)
        {
            //_fontRenderer = systems.GetShared<FontRenderer>();
        }

        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var entities = world.Filter<UIPosition>().Inc<UIText>().End();
            var positionPool = world.GetPool<UIPosition>();
            var textPool = world.GetPool<UIText>();

            foreach(var entity in entities)
            {
                ref UIPosition pos = ref positionPool.Get(entity);
                ref UIText text = ref textPool.Get(entity);

                _fontRenderer.DrawText(text.text, pos.position, 22.0f);
            }
        }
    }
}
