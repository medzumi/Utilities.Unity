using System;
using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;

[Serializable]
public class Entity
{
    public string DebugName;
    [SerializeReference] public List<IConfigurableComponent> Components = new List<IConfigurableComponent>();
}

[Serializable]
public struct DebugName : IConfigurableComponent
{
    public string Name;
}

[CreateAssetMenu]
public class EntityScene : ScriptableObject
{
    [SerializeField] private List<Entity> _entities;
    
    public void Install(EcsWorld world)
    {
        Dictionary<Type, IEcsPool> _pools = new Dictionary<Type, IEcsPool>();
        foreach (var entityData in _entities)
        {
            var entity = world.NewEntity();
            foreach (var entityComponent in entityData.Components)
            {
                var type = entityComponent.GetType();
                if (!_pools.TryGetValue(type, out var pool))
                {
                    _pools[type] = pool = world.GetPoolByType(type);
                }

                if (pool != null)
                {
                    pool.AddRaw(entity, entityComponent);
                }
            }

            world.GetPool<DebugName>().Add(entity).Name = entityData.DebugName;
        }
    }

    public void Save(List<Entity> entityComponents)
    {
        _entities = entityComponents;
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
        #endif
    }
}
