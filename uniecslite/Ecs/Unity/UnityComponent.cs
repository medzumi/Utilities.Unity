using ApplicationScripts.Ecs;
using ApplicationScripts.Ecs.Utility;
using ApplicationScripts.Properties;
using Leopotam.EcsLite;
using UnityEngine;

namespace ApplicationScripts.Logic.Features.Unity
{
    public abstract class UnityComponent : MonoBehaviour
    {
        public abstract void Initialize(EcsWorld ecsWorld, int entity);

        public abstract void Dispose();
    }

    public abstract class UnityComponent<T> : UnityComponent, IUpdatable<T> where T : struct
    {
        private readonly ReactiveProperty<bool> _hasComponent = new ReactiveProperty<bool>();
        
        protected EcsPool<ListComponent<IUpdatable<T>>> _pool { get; private set; }
        private int _entity;

        private bool _isDisposed = false;

        public int Entity => _entity;

        protected EcsWorld World
        {
            get;
            private set;
        } 
        
        public override void Initialize(EcsWorld ecsWorld, int entity)
        {
            _isDisposed = false;
            World = ecsWorld;
            _pool = ecsWorld.GetPool<ListComponent<IUpdatable<T>>>();
            _entity = entity;
            _pool.Ensure(_entity).ComponentData.Add(this);
            var componentPool = ecsWorld.GetPool<T>();
            if (componentPool.Has(entity))
            {
                _hasComponent.Value = true;
                UpdateData(componentPool.Get(entity));
            }
            else
            {
                _hasComponent.Value = false;
            }
        }

        public abstract void UpdateData(T data);
        public virtual void Clear()
        {
            
        }

        public override void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            ListComponent<IUpdatable<T>> component = default;
            if (_pool.TrySafeGet(_entity, World, out component))
            {
                component.ComponentData.Remove(this);
            }
            _hasComponent.Value = false;
            _entity = 0;
        }
    }
}