using Leopotam.EcsLite;
using UnityEngine;

namespace ApplicationScripts.Ecs.Unity
{
    public class ConvertComponentSystem<T> : IEcsPreInitSystem where T : struct
    {
        public void PreInit(EcsSystems systems)
        {
            var objects = Object.FindObjectsOfType<ConvertToEntity>();
            foreach (var convertToEntity in objects)
            {
                convertToEntity.Convert<T>(systems);
            }
        }
    }
}