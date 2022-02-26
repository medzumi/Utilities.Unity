using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.Utility;
using Leopotam.EcsLite;

namespace ApplicationScripts.Logic.Features.Unity
{
    public class UpdateUnityComponentSystem<T> : EcsReactiveSystem<T> where T :struct
    {
        private EcsPool<T> _pool1;
        private EcsPool<ListComponent<IUpdatable<T>>> _pool2;

        private collector _deleteCollector;

        public override void PreInit(EcsSystems systems)
        {
            base.PreInit(systems);
            _deleteCollector = systems.GetWorld()
                .Filter<T>()
                .Inc<ListComponent<IUpdatable<T>>>()
                .EndCollector(CollectorEvent.Removed);
            _pool1 = systems.GetWorld().GetPool<T>();
            _pool2 = systems.GetWorld().GetPool<ListComponent<IUpdatable<T>>>();
        }

        protected override EcsWorld.Mask ConfigureAdditionalMask(EcsWorld.Mask mask)
        {
            return base.ConfigureAdditionalMask(mask).Inc<ListComponent<IUpdatable<T>>>();
        }

        protected override EcsWorld GetWorld(EcsSystems systems)
        {
            return systems.GetWorld();
        }

        public override void Run(EcsSystems systems)
        {
            base.Run(systems);
            foreach (var entity in _deleteCollector)
            {
                foreach (var updatable in _pool2.Get(entity).ComponentData)
                {
                    updatable.Clear();
                }
            }
            _deleteCollector.Clear();
        }

        protected override void RunEntity(int entity)
        {
            var viewComponent = _pool2.Get(entity);
            var data = _pool1.Get(entity);
            foreach (var unityViewComponent in viewComponent.ComponentData)
            {
                unityViewComponent.UpdateData(data);
            }
        }
    }
}