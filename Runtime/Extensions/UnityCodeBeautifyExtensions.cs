using UnityEngine;
using Utilities.CodeExtensions;

namespace Utilities.Unity.Extensions
{
    public static class UnityCodeBeautifyExtensions
    {
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