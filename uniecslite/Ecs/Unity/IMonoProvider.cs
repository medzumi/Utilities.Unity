using Leopotam.EcsLite;

namespace ApplicationScripts.Ecs.Unity
{
    public interface IMonoProvider
    {
        void Convert(int entity, EcsWorld world);
    }
}