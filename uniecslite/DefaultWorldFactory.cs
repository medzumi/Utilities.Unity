using UnityEngine;

namespace Leopotam.EcsLite
{
    [CreateAssetMenu]
    public class DefaultWorldFactory : ScriptableFactory<EcsWorld>
    {
        [SerializeField] private EcsWorld.Config _config;
        
        public override EcsWorld Create()
        {
            return new EcsWorld(_config);
        }
    }
}