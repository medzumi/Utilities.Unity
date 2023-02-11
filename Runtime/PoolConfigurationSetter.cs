using medzumi.Utilities.Pooling;
using UnityEngine;

namespace medzumi.utilities.unity
{
    public static class PoolConfigurationSetter
    {
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            PoolFactory.instance.SetPoolConfiguration(new GameObjectPoolConfiguration());
            PoolFactory.instance.SetPoolConfiguration(new ObjectPoolConfiguration());
            PoolFactory.instance.SetPoolConfiguration(new TransformPoolConfiguration());
        }   
        
        private class ObjectPoolConfiguration : IPoolConfiguration<Object, Object>
        {
            public Object Create(Object tObject)
            {
                return Object.Instantiate(tObject);
            }

            public void ResolveAction(Object tObject)
            {
            }

            public void ReleaseAction(Object tObject)
            {
            }
        }
        
        private class TransformPoolConfiguration : IPoolConfiguration<Transform, Transform>
        {
            public Transform Create(Transform tObject)
            {
                return Object.Instantiate(tObject);
            }

            public void ResolveAction(Transform tObject)
            {
                tObject.gameObject.SetActive(true);
            }

            public void ReleaseAction(Transform tObject)
            {
                tObject.gameObject.SetActive(false);
            }
        }
        
        private class ComponentPoolConfiguration : IPoolConfiguration<Component, Component>
        {
            public Component Create(Component tObject)
            {
                return Object.Instantiate(tObject);
            }

            public void ResolveAction(Component tObject)
            {
                tObject.gameObject.SetActive(true);
            }

            public void ReleaseAction(Component tObject)
            {
                tObject.gameObject.SetActive(false);
            }
        }
        
        private class GameObjectPoolConfiguration : IPoolConfiguration<GameObject, GameObject>
        {
            public GameObject Create(GameObject tObject)
            {
                return Object.Instantiate(tObject);
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
}