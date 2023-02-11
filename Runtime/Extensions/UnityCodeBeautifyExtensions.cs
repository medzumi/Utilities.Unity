using medzumi.Utilities.CodeExtensions;
using UnityEngine;

namespace Utilities.Unity.Extensions
{
    public static class UnityCodeBeautifyExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent(out T result))
            {
                return result;
            }
            else
            {
                return gameObject.AddComponent<T>();
            }
        }
        
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            if (component.TryGetComponent(out T result))
            {
                return result;
            }
            else
            {
                return component.gameObject.AddComponent<T>();
            }
        }
        
        public static bool IsNullInUnity<T>(this T obj)
        {
            if (obj is Object uObj)
            {
                return !uObj;
            }
            else
            {
                return obj.IsNull();
            }
        }

        public static bool IsNotNullInUnity<T>(this T obj)
        {
            if (obj is Object uObj)
            {
                return uObj;
            }
            else
            {
                return obj.IsNotNull();
            }
        }
    }
}