using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utilities.Unity.Editor
{
    public struct CurrentFieldInfo
    {
        public readonly object fieldInfoinstance;
        public readonly FieldInfo fieldInfo;

        public CurrentFieldInfo(object fieldInfoinstance, FieldInfo fieldInfo)
        {
            this.fieldInfoinstance = fieldInfoinstance;
            this.fieldInfo = fieldInfo;
        }
    }
    
    public static class TypeExtensions
    {
        public static CurrentFieldInfo GetCurrentPropertyFieldInfo(this SerializedProperty serializedProperty)
        {
            var pathParts = serializedProperty.propertyPath.Split('.');
            var targetObject = serializedProperty.serializedObject.targetObject;
            object instance = targetObject;
            var targetType = targetObject.GetType();
            FieldInfo fi = null;
            for (int i = 0; i < pathParts.Length; i++)
            {
                fi = targetType.GetField(pathParts[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fi != null)
                {
                    if (fi.FieldType.IsArray)
                    {
                        targetType = fi.FieldType.GetElementType();
                        i += 2;
                        continue;
                    }
                    
                    targetType = fi.FieldType;
                    continue;
                }
                else
                {
                    return new CurrentFieldInfo(null, null);
                }
            }

            return new CurrentFieldInfo(null, fi);
        }

        public static bool IsAssignableTo(this Type fromType, Type type)
        {
            if (type.IsAssignableFrom(fromType))
            {
                return true;
            }
            else if (type.IsGenericTypeDefinition)
            {
                while (fromType != null)
                {
                    if (fromType.GetGenericTypeDefinition() == type)
                    {
                        return true;
                    }

                    fromType = fromType.BaseType;
                }
            }

            return false;
        }

        public static bool TryGetGenericDefines(this Type fromType, Type genericTypeDefinitiom, List<Type> types)
        {
            types.Clear();
            if (genericTypeDefinitiom.IsGenericTypeDefinition)
            {
                if (genericTypeDefinitiom.IsClass)
                {
                    while (fromType != null)
                    {
                        if (fromType.IsGenericType && fromType.GetGenericTypeDefinition() == genericTypeDefinitiom)
                        {
                            types.Add(fromType);
                            return true;
                        }
                        fromType = fromType.BaseType;
                    }
                }
                else if(genericTypeDefinitiom.IsInterface)
                {
                    foreach (var variable in fromType.GetInterfaces())
                    {
                        if (variable.IsGenericType && variable.GetGenericTypeDefinition() == genericTypeDefinitiom)
                        {
                            types.Add(variable);
                        }
                    }

                    return types.Count > 0;
                }
            }

            return false;
        }
    }
}