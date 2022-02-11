using System;
using System.Collections.Generic;
using ApplicationScripts.CodeExtensions;
using Leopotam.EcsLite;
using UnityEngine;

namespace ApplicationScripts.Ecs.Unity
{
    public class ConvertToEntity : MonoBehaviour
    {
        public string worldName;
        private EcsWorld _ecsWorld;
        
        private int _entity;

        [SerializeReference] private List<object> _list = new List<object>();

        public int Entity => _entity;

        public void Convert<T>(EcsSystems systems) where T : struct
        {
            if (_ecsWorld.IsNull())
            {
                _ecsWorld = systems.GetSafeWorld(worldName);
                _entity = _ecsWorld.NewEntity();
            }

            if (TryGetComponent(out MonoProvider<T> provider))
            {
                provider.Convert(_entity, _ecsWorld);
            }
        }

        public void ConvertFull(EcsSystems systems)
        {
            try
            {
                if (_ecsWorld.IsNull())
                {
                    _ecsWorld = systems.GetSafeWorld(worldName);
                    _entity = _ecsWorld.NewEntity();
                }

                foreach (var monoProvider in GetComponents<IMonoProvider>())
                {
                    monoProvider.Convert(_entity, _ecsWorld);
                }

                foreach (var o in _list)
                {
                    _ecsWorld.GetPoolByType(o.GetType()).AddRaw(_entity, o);
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e, this);
            }
        }
    }
}