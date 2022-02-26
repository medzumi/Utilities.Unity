using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.Utility;
using Leopotam.EcsLite;

namespace ApplicationScripts.Logic.Features.Unity
{
    public abstract class UnityCustomLogicComponent<TLogic> : UnityComponent where TLogic : class
    {
        private EcsPool<ReferenceComponent<TLogic>> _pool;
        private int _entity;
        
        protected EcsWorld world { get; private set; }
        protected abstract TLogic GetSelf { get; }
        
        public override void Initialize(EcsWorld ecsWorld, int entity)
        {
            world = ecsWorld;
            _pool = ecsWorld.GetPool<ReferenceComponent<TLogic>>();
            _entity = entity;
            _pool.Ensure(entity).reference = GetSelf;
        }

        public override void Dispose()
        {
            _pool.SafeDel(_entity, world);
        }
    }
}