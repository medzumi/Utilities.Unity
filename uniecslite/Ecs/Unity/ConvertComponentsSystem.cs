using Leopotam.EcsLite;
using UnityEngine;

namespace ApplicationScripts.Ecs.Unity
{
    public class ConvertComponentsSystem : IEcsInitSystem
    {
        public void Init(EcsSystems systems)
        {
            var objects = Object.FindObjectsOfType<ConvertToEntity>();
            foreach (var convertToEntity in objects)
            {
                convertToEntity.ConvertFull(systems);
            }
        }
    }
}