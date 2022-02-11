using Leopotam.EcsLite;

namespace ApplicationScripts.Ecs.Unity
{
    public static class ConvertExtensions
    {
        public static EcsSystems ConvertComponent<T>(this EcsSystems systems) where T : struct
        {
            systems.Add(new ConvertComponentSystem<T>());
            return systems;
        }

        public static EcsSystems ConvertComponents(this EcsSystems systems)
        {
            systems.Add(new ConvertComponentsSystem());
            return systems;
        }
    }
}