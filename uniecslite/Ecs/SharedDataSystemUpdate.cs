using ApplicationScripts.Ecs.Utility;
using ApplicationScripts.Logic.Features.Unity;
using Leopotam.EcsLite;

namespace ApplicationScripts.Ecs
{
    public abstract class SharedDataSystemUpdate<TComponent> : EcsReactiveSystem<TComponent> where TComponent : struct
    {
        private EcsPool<TComponent> _componentPool;
        private EcsPool<ReferenceComponent<UnitySharedComponent<TComponent>>> _pool;
        private EcsFilter _viewComponentFilter;
        private EcsFilter _executeFilter;
        private bool _hasComponent;

        public override void PreInit(EcsSystems systems)
        {
            base.PreInit(systems);
            var world = systems.GetWorld();
            _componentPool = world.GetPool<TComponent>();
            _pool = world.GetPool<ReferenceComponent<UnitySharedComponent<TComponent>>>();
            _viewComponentFilter = world.Filter<ReferenceComponent<UnitySharedComponent<TComponent>>>().End();
            _executeFilter = world.Filter<TComponent>().End();
        }

        public override void Run(EcsSystems systems)
        {
            if (!_hasComponent && _executeFilter.GetEntitiesCount() > 0)
            {
                _hasComponent = true;
                foreach (var entity in _viewComponentFilter)
                {
                    _pool.Get(entity).reference.SetHasComponent(true);
                }
            }

            if (_hasComponent && _executeFilter.GetEntitiesCount() == 0)
            {
                _hasComponent = false;
                foreach (var entity in _viewComponentFilter)
                {
                    _pool.Get(entity).reference.SetHasComponent(false);
                }
            }
            base.Run(systems);
        }

        protected override void RunEntity(int entity)
        {
            var data = _componentPool.Get(entity);
            foreach (var viewEntity in _viewComponentFilter)
            {
                var reference = _pool.Get(viewEntity).reference;
                reference.UpdateComponent(data);
            }
        }
    }
}