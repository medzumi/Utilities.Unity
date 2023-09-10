using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using medzumi.Utilities;
using UnityEditor;
using UnityEngine;

namespace Utilities.Unity.Editor
{
    public struct CurrentFieldInfo
    {
        public readonly object FieldValue;
        public readonly Type FieldType;
        public readonly FieldInfo FieldInfo;

        public CurrentFieldInfo(object fieldValue, Type fieldType, FieldInfo fieldInfo)
        {
            this.FieldValue = fieldValue;
            this.FieldType = fieldType;
            FieldInfo = fieldInfo;
        }
    }

    public static class EditorTypeExtensions
    {
        public struct PropertyHandlerData
        {
            public SerializedProperty SerializedProperty;
            public string[] Path;
            public int CurrentIndex;
            public Type FieldType;
            public Type ValueType;
            public object Value;
            public FieldInfo FieldInfo;
        }

        public static readonly Dictionary<Type, Func<PropertyHandlerData, PropertyHandlerData>> GetPropertyHandlers =
            new Dictionary<Type, Func<PropertyHandlerData, PropertyHandlerData>>()
            {
                {
                    typeof(List<>), propertyHandlerData =>
                    {
                        propertyHandlerData.SerializedProperty =
                            propertyHandlerData.SerializedProperty.FindPropertyRelative(
                                propertyHandlerData.Path[propertyHandlerData.CurrentIndex]);
                        propertyHandlerData.CurrentIndex++;
                        var arrayIndex = int.Parse(propertyHandlerData.Path[propertyHandlerData.CurrentIndex]
                            .Trim('d', 'a', 't', 'a', '[', ']'));
                        propertyHandlerData.SerializedProperty =
                            propertyHandlerData.SerializedProperty.FindPropertyRelative(
                                propertyHandlerData.Path[propertyHandlerData.CurrentIndex]);
                        propertyHandlerData.CurrentIndex++;
                        while (!propertyHandlerData.FieldType.IsGenericType &&
                               propertyHandlerData.FieldType.GetGenericTypeDefinition() != typeof(List<>))
                        {
                            propertyHandlerData.FieldType = propertyHandlerData.FieldType.BaseType;
                        }

                        propertyHandlerData.FieldType = propertyHandlerData.FieldType.GetGenericArguments()[0];
                        propertyHandlerData.Value = (propertyHandlerData.Value as IList)[arrayIndex];
                        propertyHandlerData.ValueType = propertyHandlerData.Value?.GetType();
                        return propertyHandlerData;
                    }
                },
                {
                    typeof(Array), propertyHandlerData =>
                    {
                        propertyHandlerData.SerializedProperty =
                            propertyHandlerData.SerializedProperty.FindPropertyRelative(
                                propertyHandlerData.Path[propertyHandlerData.CurrentIndex]);
                        propertyHandlerData.CurrentIndex++;
                        var arrayIndex = int.Parse(propertyHandlerData.Path[propertyHandlerData.CurrentIndex]
                            .Trim('d', 'a', 't', 'a', '[', ']'));
                        propertyHandlerData.SerializedProperty =
                            propertyHandlerData.SerializedProperty.FindPropertyRelative(
                                propertyHandlerData.Path[propertyHandlerData.CurrentIndex]);
                        propertyHandlerData.CurrentIndex++;
                        propertyHandlerData.FieldType = propertyHandlerData.FieldType.GetElementType();
                        propertyHandlerData.Value = (propertyHandlerData.Value as IList)[arrayIndex];
                        propertyHandlerData.ValueType = propertyHandlerData.Value?.GetType();
                        return propertyHandlerData;
                    }
                }
            };

        public static CurrentFieldInfo GetPropertyTypes(this SerializedProperty serializedProperty)
        {
            var pathElements = serializedProperty.propertyPath.Split('.');
            var propertyHandlerData = new PropertyHandlerData()
            {
                Path = pathElements,
                CurrentIndex = 0,
                FieldType = null,
                ValueType = serializedProperty.serializedObject.targetObject.GetType(),
                Value = serializedProperty.serializedObject.targetObject,
                SerializedProperty = null
            };
            do
            {
                if (GetPropertyHandlers.TryFindWithRecursiveType(propertyHandlerData.FieldType,
                        out var propertyHandler))
                {
                    propertyHandlerData = propertyHandler.Invoke(propertyHandlerData);
                }
                else
                {
                    propertyHandlerData.SerializedProperty = propertyHandlerData.SerializedProperty != null
                        ? propertyHandlerData
                            .SerializedProperty
                            .FindPropertyRelative(
                                propertyHandlerData.Path[propertyHandlerData.CurrentIndex])
                        : serializedProperty.serializedObject
                            .FindProperty(propertyHandlerData.Path[propertyHandlerData.CurrentIndex]);
                    propertyHandlerData.FieldInfo = propertyHandlerData.ValueType.GetFieldInfoRecursive(
                        propertyHandlerData.Path[propertyHandlerData.CurrentIndex]);
                    propertyHandlerData.FieldType = propertyHandlerData.FieldInfo?.FieldType;
                    propertyHandlerData.Value = propertyHandlerData.FieldInfo?.GetValue(propertyHandlerData.Value);
                    propertyHandlerData.ValueType = propertyHandlerData.Value?.GetType();
                    propertyHandlerData.CurrentIndex++;
                }
            } while (propertyHandlerData.CurrentIndex < pathElements.Length);

            return new CurrentFieldInfo(propertyHandlerData.Value, propertyHandlerData.FieldType, propertyHandlerData.FieldInfo);
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
                else if (genericTypeDefinitiom.IsInterface)
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