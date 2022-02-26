using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.Utility;
using ApplicationScripts.Properties;
using Leopotam.EcsLite;

namespace ApplicationScripts.Logic.Features.Unity
{
    public abstract class UnitySharedComponent<T> : UnityComponent
    {
        private EcsPool<ReferenceComponent<UnitySharedComponent<T>>> _pool;
        private readonly ReactiveProperty<bool> _hasComponent = new ReactiveProperty<bool>();
        private int _entity;
        protected int _sharedEntity { get; private set; }
        
        public override void Initialize(EcsWorld ecsWorld, int entity)
        {
            _pool = ecsWorld.GetPool<ReferenceComponent<UnitySharedComponent<T>>>();
            _entity = entity;
            _pool.Ensure(entity).reference = this;
        }

        public abstract void UpdateComponent(T data);

        public void SetSharedEntity(int entity)
        {
            _sharedEntity = entity;
        }

        public void SetHasComponent(bool has)
        {
            _hasComponent.Value = has;
        }

        public override void Dispose()
        {
            _pool.Del(_entity);
        }
    }
}