using System;
using System.Collections;
using System.Collections.Generic;
using ApplicationScripts.Unity;
using UnityEngine;

namespace Leopotam.EcsLite
{
    public interface IWorldEcsSystem : IEcsSystem
    {
        void SetWorldName(string worldName);
    }

    public abstract class ScriptableFactory<T> : ScriptableObject
    {
        public abstract T Create();
    }

    [Serializable]
    public struct WorldConfiguration
    {
        public string WorldName;
        public ScriptableFactory<EcsWorld> WorldFactory;
        public List<ScriptableFactory<IWorldEcsSystem>> WorldEcsSystemsFactories;
    }
    
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private WorldConfiguration _defaultWorldConfiguration;
        [SerializeField] private WorldConfiguration[] _worldConfigurations;
        [SerializeField] private List<ScriptableFactory<IEcsSystem>> _scriptableSystemFactories;

        private EcsSystems _ecsSystems;

        protected EcsSystems EcsSystems => _ecsSystems;
        
        [Button(Name = "Reload")]
        protected virtual void Reload()
        {
            var world = _defaultWorldConfiguration.WorldFactory.Create();
            _ecsSystems = new EcsSystems(world);
            foreach (var factory in _defaultWorldConfiguration.WorldEcsSystemsFactories)
            {
                var system = factory.Create();
                system.SetWorldName(_defaultWorldConfiguration.WorldName);
                _ecsSystems.Add(system);
            }

            foreach (var worldConfiguration in _worldConfigurations)
            {
                _ecsSystems.AddWorld(worldConfiguration.WorldFactory.Create(), worldConfiguration.WorldName);
                foreach (var factory in worldConfiguration.WorldEcsSystemsFactories)
                {
                    var system = factory.Create();
                    system.SetWorldName(worldConfiguration.WorldName);
                    _ecsSystems.Add(system);
                }
            }

            foreach (var factory in _scriptableSystemFactories)
            {
                _ecsSystems.Add(factory.Create());
            }
        }

        protected virtual void Update()
        {
            _ecsSystems?.Run();
        }

        protected virtual void OnDestroy()
        {
            _ecsSystems?.Destroy();
        }
    }
}
