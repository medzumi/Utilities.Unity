using System.Collections.Generic;
using Leopotam.EcsLite;

namespace ApplicationScripts.Ecs
{
    public class EcsSystemBase : IEcsPreInitSystem, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem, IEcsPostDestroySystem
    {
        public virtual void PreInit(EcsSystems systems)
        {
        }

        public virtual void Init(EcsSystems systems)
        {
        }

        public virtual void Run(EcsSystems systems)
        {
        }

        public virtual void Destroy(EcsSystems systems)
        {
        }

        public virtual void PostDestroy(EcsSystems systems)
        {
        }
    }

    public class EncapsulateSystemCollection : EcsSystemBase
    {
        private readonly List<IEcsSystem> _systems = new List<IEcsSystem>();
        private readonly List<IEcsRunSystem> _runSystems = new List<IEcsRunSystem>();

        protected EncapsulateSystemCollection ProtectedAdd(IEcsSystem system)
        {
            _systems.Add(system);
            if (system is IEcsRunSystem runSystem)
            {
                _runSystems.Add(runSystem);
            }

            return this;
        }

        public override void PreInit(EcsSystems systems)
        {
            base.PreInit(systems);
            _systems.PreInitBindly(systems);
        }

        public override void Init(EcsSystems systems)
        {
            base.Init(systems);
            _systems.InitBindly(systems);
        }

        public override void Run(EcsSystems systems)
        {
            base.Run(systems);
            _runSystems.Run(systems);
        }

        public override void Destroy(EcsSystems systems)
        {
            base.Destroy(systems);
            _systems.DestroyBindly(systems);
        }

        public override void PostDestroy(EcsSystems systems)
        {
            base.PostDestroy(systems);
            _systems.PostDestroyBindly(systems);
        }
    }
    
    public class SystemCollection : EncapsulateSystemCollection
    {
        public SystemCollection Add(IEcsSystem system)
        {
            ProtectedAdd(system);
            return this;
        }
    }
}