using medzumi.Utilities.Pooling;
using UnityEngine;
using UnityEngine.Scripting;

namespace medzumi.utilities.unity
{
    [Preserve]
    public static class PoolConfigurationSetter
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        [Preserve]
        private static void Initialize()
        {
            PoolExtensions.PoolFactory.SetPoolConfiguration(new GameObjectPoolConfiguration());
            PoolExtensions.PoolFactory.SetPoolConfiguration(new ObjectPoolConfiguration());
        }   
        
        private class ObjectPoolConfiguration : IPoolConfiguration<Object, Object>
        {
            public Object Create(Object tObject, IPoolReleaser<Object> poolReleaser)
            {
                var inst = Object.Instantiate(tObject);
                inst.name = tObject.name;
                return inst;
            }

            public void ResolveAction(Object tObject)
            {
            }

            public void ReleaseAction(Object tObject)
            {
            }
        }

        public class PoolHandler : MonoBehaviour
        {
            public IPoolReleaser<GameObject> Pool;
        }

        private class GameObjectPoolConfiguration : IPoolConfiguration<GameObject, GameObject>
        {
            public GameObject Create(GameObject tObject, IPoolReleaser<GameObject> poolReleaser)
            {
                var obj = Object.Instantiate(tObject);
                obj.AddComponent<PoolHandler>()
                    .Pool = poolReleaser;
                obj.name = tObject.name;
                return obj;
            }

            public void ResolveAction(GameObject tObject)
            {
                tObject.SetActive(true);
            }

            public void ReleaseAction(GameObject tObject)
            {
                tObject.SetActive(false);
            }
        }
    }

    public static class UnityPoolExtensions
    {
        public static IPool<T> GetPool<T>(this T tObject) where T : Component
        {
            var poolGo = tObject.gameObject.GetPool();
            return new FakePool<T>(poolGo, () => poolGo.Get().GetComponent<T>(),
                component => poolGo.Release(component.gameObject));
        }

        public static void Release<T>(this T tObject) where T : Component
        {
            if (tObject.TryGetComponent(out PoolConfigurationSetter.PoolHandler poolHandler))
            {
                poolHandler.Pool.Release(tObject.gameObject);
            }
        }

        public static void Release(this GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out PoolConfigurationSetter.PoolHandler poolHandler))
            {
                poolHandler.Pool.Release(gameObject.gameObject);
            }
        }
    }
}