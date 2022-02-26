using System.Collections.Generic;
using Leopotam.EcsLite;

namespace ApplicationScripts.Ecs
{
    public abstract class EcsReactiveSystem<T> : IEcsRunSystem, IEcsPreInitSystem where T : struct
    {
        protected collector _collector;

        protected abstract EcsWorld GetWorld(EcsSystems systems);

        protected virtual EcsWorld.Mask ConfigureAdditionalMask(EcsWorld.Mask mask)
        {
            return mask;
        }

        public virtual void Run(EcsSystems systems)
        {
            foreach (var entity in _collector)
            {
                RunEntity(entity);
            }
            _collector.Clear();
        }

        protected abstract void RunEntity(int entity);

        public virtual void PreInit(EcsSystems systems)
        {
            _collector = ConfigureAdditionalMask(GetWorld(systems).Filter<T>()).EndCollector(CollectorEvent.Added | CollectorEvent.Dirt);
        }
    }
}