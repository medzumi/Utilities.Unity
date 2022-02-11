using System;
using System.Collections.Generic;
using ApplicationScripts.Ecs;
using ApplicationScripts.Unity;
using UnityEngine;

namespace Leopotam.EcsLite
{
    [ExecuteAlways]
    public class EditorEntryPoint : EntryPoint
    {
        [SerializeField] private EntityScene _entityScene;

        private IEcsPool[] _ecsPools = new IEcsPool[0];
        private int[] _entities = new int[0];
        
        [Button(Name = "Load")]
        private void Load()
        {
            if (EcsSystems != null)
            {
                foreach (var keyValuePair in EcsSystems.GetAllNamedWorlds())
                {
                    int count = keyValuePair.Value.GetAllPools(ref _ecsPools);
                    int entitiesCount = keyValuePair.Value.GetAllEntities(ref _entities);

                    for (int i = 0; i < count; i++)
                    {
                        var pool = _ecsPools[i];
                        for (int j = 0; j < entitiesCount; j++)
                        {
                            if (pool.Has(_entities[j]))
                            {
                                pool.Del(_entities[j]);
                            }
                        }
                    }
                }
                
                _entityScene.Install(EcsSystems.GetWorld());
            }
        }

        [Button(Name = "Save")]
        private void Save()
        {
            var world = EcsSystems.GetWorld();
            var entitiesCount = world.GetAllEntities(ref _entities);
            var poolCount = world.GetAllPools(ref _ecsPools);

            var entitiesComponent = new List<Entity>();

            var interfaceType = typeof(IConfigurableComponent);
            for (int i = 0; i < entitiesCount; i++)
            {
                var entityComponents = new List<IConfigurableComponent>();
                for (int j = 0; j < poolCount; j++)
                {
                    var poolType = _ecsPools[j].GetComponentType();
                    if (interfaceType.IsAssignableFrom(poolType) && poolType != typeof(DebugName))
                    {
                        if (_ecsPools[j].Has(_entities[i]))
                        {
                            entityComponents.Add(_ecsPools[j].GetRaw(_entities[i]) as IConfigurableComponent);
                        }
                    }
                }

                entitiesComponent.Add(new Entity()
                {
                    DebugName = world.GetPool<DebugName>().Ensure(_entities[i]).Name,
                    Components = entityComponents
                });
            }
            _entityScene.Save(entitiesComponent);
        }
    }
}