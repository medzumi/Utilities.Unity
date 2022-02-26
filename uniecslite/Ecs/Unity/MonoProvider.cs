using Leopotam.EcsLite;
using UnityEngine;

namespace ApplicationScripts.Ecs.Unity
{
    public abstract class MonoProvider : MonoBehaviour, IMonoProvider
    {
        public abstract void Convert(int entity, EcsWorld world);
    }
    
    public abstract class MonoProvider<T> : MonoProvider where T :struct
    {
        [SerializeField] private T _component;

        public override void Convert(int entity, EcsWorld world)
        {
            var pool = world.GetPool<T>();
            if (!pool.Has(entity))
            {
                pool.Add(entity) = _component;
            }
        }
    }
}