using System;
using System.Collections.Generic;
using Leopotam.EcsLite;

namespace ApplicationScripts.Ecs
{
    public static class EcsExtensions
    {
        private static IEcsSystem[] _systems = new IEcsSystem[0];
        
        public static ref T Ensure<T>(this EcsPool<T> pool, int entity) where T : struct
        {
            if (pool.Has(entity))
            {
                return ref pool.Set(entity);
            }
            else
            {
                return ref pool.Add(entity);
            }
        }

        public static TSystem GetSystem<TSystem>(this EcsSystems systems) where TSystem : IEcsSystem
        {
            var count = systems.GetAllSystems(ref _systems);
            var genericType = typeof(TSystem);
            for (int i = 0; i < count; i++)
            {
                if (_systems[i].GetType() == genericType)
                    return (TSystem)_systems[i];
            }

            return default;
        }

        public static EcsSystems AddDynamicSystem<TDynamicSystem>(this EcsSystems systems) where TDynamicSystem : class, IEcsSystem, new()
        {
            systems.GetSystem<DynamicInitializingSystems>().Add<TDynamicSystem>();
            return systems;
        }

        public static void SafeDel<T>(this EcsPool<T> pool, int entity) where T : struct
        {
            if(pool.Has(entity))
                pool.Del(entity);
        }

        public static void SafeDel<T>(this EcsPool<T> pool, int entity, EcsWorld world) where T : struct
        {
            if (world.PackEntity(entity).Unpack(world, out entity))
            {
                pool.SafeDel(entity);
            }
        }

        public static bool TrySafeGet<T>(this EcsPool<T> pool, int entity, EcsWorld world, out T component) where T : struct
        {
            component = default;
            if (world.PackEntity(entity).Unpack(world, out entity))
            {
                component = pool.Get(entity);
                return true;
            }

            return false;
        }
        
        public struct SystemCollectionConfigurator<TSystem> where TSystem : IEcsSystems
        {
            private readonly TSystem _collection;

            public SystemCollectionConfigurator(TSystem collection)
            {
                _collection = collection;
            }

            public SystemCollectionConfigurator<TSystem> Add<T>() where T : IEcsSystem, new()
            {
                _collection.Add(new T());
                return this;
            }

            public TSystem End()
            {
                return _collection;
            }
        }
        public static SystemCollectionConfigurator<T> StartConfigure<T>(this T system) where T : IEcsSystems
        {
            return new SystemCollectionConfigurator<T>(system);
        }

        public static IEcsSystems Add<TSystem>(this IEcsSystems systems) where TSystem : IEcsSystem, new()
        {
            systems.Add(new TSystem());
            return systems;
        }

        public static IEcsSystems Add(this IEcsSystems systems, IEcsSystem system)
        {
            systems.Add(system);
            return systems;
        }

        public static EcsWorld GetSafeWorld(this EcsSystems systems, string name = "")
        {
            return systems.GetWorld(string.IsNullOrWhiteSpace(name) ? null : name);
        }

        public static EcsSystems OneFrame<TOneFrameComponent>(this EcsSystems systems, string worldName = "")
            where TOneFrameComponent : struct
        {
            systems.Add(new OneFrameSystem<TOneFrameComponent>(worldName));
            return systems;
        }

        public static T PreInitBindly<T>(this T system, EcsSystems systems) where T : IEcsSystem
        {
            if (system is IEcsPreInitSystem preInitSystem)
            {
                preInitSystem.PreInit(systems);
            }

            return system;
        }

        public static T InitBindly<T>(this T system, EcsSystems systems) where T : IEcsSystem
        {
            if (system is IEcsInitSystem initSystem)
            {
                initSystem.Init(systems);
            }

            return system;
        }

        public static T DestroyBindly<T>(this T system, EcsSystems systems) where T : IEcsSystem
        {
            if (system is IEcsDestroySystem destroySystem)
            {
                destroySystem.Destroy(systems);
            }

            return system;
        }

        public static T PostDestroyBindly<T>(this T system, EcsSystems systems) where T : IEcsSystem
        {
            if (system is IEcsPostDestroySystem postDestroySystem)
            {
                postDestroySystem.PostDestroy(systems);
            }

            return system;
        }

        public static List<T> PreInitBindly<T>(this List<T> listSystems, EcsSystems systems) where T : IEcsSystem
        {
            foreach (var ecsSystem in listSystems)
            {
                ecsSystem.PreInitBindly(systems);
            }

            return listSystems;
        }
        
        public static List<T> InitBindly<T>(this List<T> listSystems, EcsSystems systems) where T : IEcsSystem
        {
            foreach (var ecsSystem in listSystems)
            {
                ecsSystem.InitBindly(systems);
            }

            return listSystems;
        }
        
        public static List<T> DestroyBindly<T>(this List<T> listSystems, EcsSystems systems) where T : IEcsSystem
        {
            foreach (var ecsSystem in listSystems)
            {
                ecsSystem.DestroyBindly(systems);
            }

            return listSystems;
        }
        
        public static List<T> PostDestroyBindly<T>(this List<T> listSystems, EcsSystems systems) where T : IEcsSystem
        {
            foreach (var ecsSystem in listSystems)
            {
                ecsSystem.PostDestroyBindly(systems);
            }

            return listSystems;
        }

        public static T RunBindly<T>(this T system, EcsSystems systems)
        {
            if (system is IEcsRunSystem runSystem)
            {
                runSystem.Run(systems);
            }

            return system;
        }

        public static List<T> RunBindly<T>(this List<T> ecsSystems, EcsSystems systems) where T : IEcsSystem
        {
            foreach (var ecsSystem in ecsSystems)
            {
                ecsSystem.RunBindly(systems);
            }

            return ecsSystems;
        }

        public static List<T> Run<T>(this List<T> runSystems, EcsSystems systems) where T :IEcsRunSystem
        {
            foreach (var ecsRunSystem in runSystems)
            {
                ecsRunSystem.Run(systems);
            }

            return runSystems;
        }

        public static EcsPool<T> Clear<T>(this EcsPool<T> pool) where T : struct
        {
            var sparse = pool.GetRawSparseItems();
            var i = 0;
            foreach (var hash in sparse)
            {
                if (hash > 0)
                {
                    pool.Del(i);
                }

                i++;
            }

            return pool;
        }
    }
}