using System;
using Leopotam.EcsLite;

namespace ApplicationScripts.Ecs
{
    public static class EcsExtensions
    {
        private static IEcsSystem[] _systems = new IEcsSystem[0];
        
        public static T Ensure<T>(this EcsPool<T> pool, int entity) where T : struct
        {
            if (pool.Has(entity))
            {
                return pool.Get(entity);
            }
            else
            {
                return pool.Add(entity);
            }
        }

        public static ref T EnsureSet<T>(this EcsPool<T> pool, int entity) where T : struct
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

        public static bool TrySafeSet<T>(this EcsPool<T> pool, int entity, EcsWorld world, ref T component) where T : struct
        {
            if (world.PackEntity(entity).Unpack(world, out entity))
            {
                component = ref pool.Set(entity);
                return true;
            }

            return false;
        }
        
        public static bool TrySafeGet<T>(this EcsPool<T> pool, int entity, EcsWorld world, out T component) where T : struct
        {
            component = default;
            
            if (world.PackEntity(entity).Unpack(world, out entity))
            {
                component = pool.Set(entity);
                return true;
            }

            return false;
        }

        public static Systems Add(this Systems systems, IEcsSystem system)
        {
            systems.AddInternal(system);
            return systems;
        }

        public static Systems Add<TSystem>(this Systems systems) where TSystem : IEcsSystem, new()
        {
            return systems.Add(new TSystem());
        }

        public static EcsSystems Add<TSystem>(this EcsSystems systems) where TSystem : IEcsSystem, new()
        {
            systems.Add(new TSystem());
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

        public static Systems OneFrame<TOneFrameComponent>(this Systems systems, string worldName = "") where TOneFrameComponent:struct
        {
            systems.Add(new OneFrameSystem<TOneFrameComponent>(worldName));
            return systems;
        }
    }
}