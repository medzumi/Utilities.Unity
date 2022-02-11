using System;
using System.Collections.Generic;
using ApplicationScripts.CodeExtensions;
using Leopotam.EcsLite;

namespace ApplicationScripts.Ecs
{
    public class DynamicInitializingSystems : IEcsPreInitSystem, IEcsPostDestroySystem, IEcsRunSystem, IEcsInitSystem
    {
        private EcsSystems _systems;
        
        private readonly Dictionary<Type, IEcsSystem> _dynamicSystems = new Dictionary<Type, IEcsSystem>();
        private readonly List<IEcsRunSystem> _runSystems = new List<IEcsRunSystem>();
        
        public void PreInit(EcsSystems systems)
        {
            _systems = systems;
            foreach (var dynamicSystem in _dynamicSystems)
            {
                if(dynamicSystem.Value is IEcsPreInitSystem preInitSystem)
                    preInitSystem.PreInit(systems);
            }
        }

        public void Init(EcsSystems systems)
        {
            foreach (var valuePair in _dynamicSystems)
            {
                if (valuePair.Value is IEcsInitSystem initSystem)
                {
                    initSystem.Init(_systems);
                }
            }
        }

        public void PostDestroy(EcsSystems systems)
        {
            foreach (var dynamicSystem in _dynamicSystems)
            {
                if (dynamicSystem.Value is IEcsPostDestroySystem postDestroySystem)
                {
                    postDestroySystem.PostDestroy(systems);
                }
            }
        }

        public void Run(EcsSystems systems)
        {
            foreach (var ecsRunSystem in _runSystems)
            {
                ecsRunSystem.Run(systems);
            }
        }

        public void Add<TDynamic>() where TDynamic : class, IEcsSystem, new()
        {
            var type = typeof(TDynamic);
            if (!_dynamicSystems.ContainsKey(type))
            {
                var dynamicSystem = new TDynamic();
                _dynamicSystems[type] = dynamicSystem;

                if (_systems.IsNotNull())
                {
                    if (dynamicSystem is IEcsPreInitSystem preInitSystem)
                    {
                        preInitSystem.PreInit(_systems);
                    }

                    if (dynamicSystem is IEcsInitSystem initSystem)
                    {
                        initSystem.Init(_systems);
                    }
                }
                if (dynamicSystem is IEcsRunSystem runSystem)
                {
                    _runSystems.Add(runSystem);
                }
            }
        }
    }
}