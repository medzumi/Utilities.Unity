using System.Collections.Generic;
using Leopotam.EcsLite;

namespace ApplicationScripts.Ecs
{
    public struct ComponentUpdated<T> where T : struct
    {
        
    }

    public interface IEcsSystemWithFlag : IEcsSystem
    {
        List<KeyValuePair<IEcsPool, EcsFilter>> GetFlagConfigurations(List<KeyValuePair<IEcsPool, EcsFilter>> configurations, EcsSystems systems);
    }

    public class Systems : IEcsInitSystem, IEcsRunSystem, IEcsPreInitSystem, IEcsDestroySystem, IEcsPostDestroySystem
    {
        private readonly List<IEcsInitSystem> _initSystems = new List<IEcsInitSystem>();
        private readonly List<IEcsPreInitSystem> _preInitSystems = new List<IEcsPreInitSystem>();
        private readonly List<IEcsRunSystem> _runSystems = new List<IEcsRunSystem>();
        private readonly List<IEcsDestroySystem> _destroySystems = new List<IEcsDestroySystem>();
        private readonly List<IEcsPostDestroySystem> _postDestroySystems = new List<IEcsPostDestroySystem>();
        private readonly List<IEcsSystemWithFlag> _systemWithFlags = new List<IEcsSystemWithFlag>();
        private bool _isInited;

        private readonly Dictionary<int, KeyValuePair<IEcsPool, EcsFilter>> _dictionary =
            new Dictionary<int, KeyValuePair<IEcsPool, EcsFilter>>();

        public void AddInternal(IEcsSystem system)
        {
            if (system is IEcsPreInitSystem preInitSystem)
            {
                _preInitSystems.Add(preInitSystem);
            }

            if (system is IEcsInitSystem initSystem)
            {
                _initSystems.Add(initSystem);
            }

            if (system is IEcsRunSystem runSystem)
            {
                _runSystems.Add(runSystem);
            }

            if (system is IEcsDestroySystem destroySystem)
            {
                _destroySystems.Add(destroySystem);
            }

            if (system is IEcsPostDestroySystem postDestroySystem)
            {
                _postDestroySystems.Add(postDestroySystem);
            }
            
            if (system is IEcsSystemWithFlag systemWithFlag)
            {
                _systemWithFlags.Add(systemWithFlag);
            }
        }

        public virtual void Init(EcsSystems systems)
        {
            foreach (var initSystem in _initSystems)
            {
                initSystem.Init(systems);
            }
            
            var buffer = new List<KeyValuePair<IEcsPool, EcsFilter>>();
            foreach (var systemWithFlag in _systemWithFlags)
            {
                buffer.Clear();
                foreach (var keyValuePair in systemWithFlag.GetFlagConfigurations(buffer, systems))
                {
                    if (!_dictionary.ContainsKey(keyValuePair.Key.GetId()))
                    {
                        _dictionary.Add(keyValuePair.Key.GetId(), keyValuePair);
                    }
                }
            }
        }

        public virtual void Run(EcsSystems systems)
        {
            foreach (var runSystem in _runSystems)
            {
                runSystem.Run(systems);
            }

            foreach (var keyValuePair in _dictionary)
            {
                var filter = keyValuePair.Value.Value;
                var pool = keyValuePair.Value.Key;
                foreach (var entity in filter)
                {
                    pool.Del(entity);
                }
            }
        }

        public virtual void PreInit(EcsSystems systems)
        {
            foreach (var preInitSystem in _preInitSystems)
            {
                preInitSystem.PreInit(systems);
            }
        }

        public virtual void Destroy(EcsSystems systems)
        {
            foreach (var destroySystem in _destroySystems)
            {
                destroySystem.Destroy(systems);
            }
        }

        public virtual void PostDestroy(EcsSystems systems)
        {
            foreach (var postDestroySystem in _postDestroySystems)
            {
                postDestroySystem.PostDestroy(systems);
            }
        }
    }

    public abstract class CollectorSystem : IEcsRunSystem, IEcsPreInitSystem
    {
        private Collector _collector;

        public abstract Collector GetCollector(EcsSystems systems);
        
        public virtual void Run(EcsSystems systems)
        {
            foreach (var entity in _collector)
            {
                RunEntity(entity);
            }
            _collector.Clear();
        }

        public abstract void RunEntity(int entity);

        public virtual void PreInit(EcsSystems systems)
        {
            _collector = GetCollector(systems);
        }
    }
    
    public abstract class EcsReactiveSystem<T> : IEcsRunSystem, IEcsSystemWithFlag, IEcsPreInitSystem where T : struct
    {
        protected EcsFilter _executeFilter { get; private set; }
        protected Collector _addCollector;

        protected abstract EcsWorld GetWorld(EcsSystems systems);

        protected virtual EcsWorld.Mask ConfigureAdditionalMask(EcsWorld.Mask mask)
        {
            return mask;
        }

        public virtual void Run(EcsSystems systems)
        {
            foreach (var entity in _addCollector)
            {
                RunEntity(entity);
            }
            foreach (var entity in _executeFilter)
            {
                RunEntity(entity);
            }
            _addCollector.Clear();
        }

        protected abstract void RunEntity(int entity);

        public List<KeyValuePair<IEcsPool, EcsFilter>> GetFlagConfigurations(List<KeyValuePair<IEcsPool, EcsFilter>> configurations, EcsSystems systems)
        {
            var world = GetWorld(systems);
            configurations.Add(new KeyValuePair<IEcsPool, EcsFilter>(world.GetPool<ComponentUpdated<T>>(), world.Filter<ComponentUpdated<T>>().End()));
            return configurations;
        }

        public virtual void PreInit(EcsSystems systems)
        {
            _addCollector = ConfigureAdditionalMask(GetWorld(systems).Filter<T>()).End().GetCollector(CollectorEvent.Added);
            _executeFilter = ConfigureAdditionalMask(GetWorld(systems).Filter<ComponentUpdated<T>>()).End();
        }
    }
}