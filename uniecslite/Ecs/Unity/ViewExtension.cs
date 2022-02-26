using ApplicationScripts.Ecs;
using Leopotam.EcsLite;

namespace ApplicationScripts.Logic.Features.Unity
{
    public static class ViewExtension
    {
        public static EcsSystems UpdateViews<T>(this EcsSystems systems) where T : struct
        {
            systems.Add(new UpdateUnityComponentSystem<T>());
            return systems;
        }

        public static SystemCollection UpdateViews<T>(this SystemCollection systemCollection) where T :struct
        {
            systemCollection.Add(new UpdateUnityComponentSystem<T>());
            return systemCollection;
        }
    }
}