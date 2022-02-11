using Leopotam.EcsLite;

namespace ApplicationScripts.Ecs
{
    public class OneFrameSystem<TOneFrameComponent> : IEcsPreInitSystem, IEcsRunSystem where TOneFrameComponent : struct
    {
        private readonly string _worldName;
        
        private EcsPool<TOneFrameComponent> _pool;
        private EcsFilter _filter;

        public OneFrameSystem()
        {
            _worldName = string.Empty;
        }
        
        public OneFrameSystem(string worldName)
        {
            _worldName = worldName;
        }

        public void PreInit(EcsSystems systems)
        {
            var world = systems.GetSafeWorld(_worldName);
            _pool = world.GetPool<TOneFrameComponent>();
            _filter = world.Filter<TOneFrameComponent>().End();
        }

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                _pool.Del(entity);
            }
        }
    }
}