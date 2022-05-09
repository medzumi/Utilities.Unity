using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utilities.CodeExtensions;
using Utilities.Unity.SerializeReferencing;

namespace Utilities.Unity.Editor.SerializeReferencing
{
    [CustomPropertyDrawer(typeof(SerializeTypesAttribute), true)]
    public class SerializeReferenceEditor : PropertyDrawer
    {
        private static float _height;
        private static GUIContent _label;

        static SerializeReferenceEditor()
        {
            _label = new GUIContent("Select Type");
            _height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, _label);
        }
        
        private string[] _keys;
        private Type[] _types;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label) +
                   EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, label) + 5f;
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var drawPosition = position;
            if (_types.IsNull())
            {
                var attr = attribute as SerializeTypesAttribute;
                var baseType = attr.InheritType;
                if (baseType.IsNull())
                {
                    //ToDo : fix me please
                    baseType = Type.GetType(property.managedReferenceFieldTypename);
                }

                IEnumerable<Type> typeLinq = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes());
                if (baseType.IsInterface)
                {
                    typeLinq = typeLinq
                        .Where(type => baseType.IsAssignableFrom(type) && !type.IsGenericType && !type.IsAbstract)
                        .ToArray();
                }
                else
                {
                    typeLinq = typeLinq
                        .Where(type => type.CheckSelfOrBaseType(baseType));
                }
                _keys = typeLinq.Select(type => type.FullName.Replace('.', '/'))
                    .ToArray();
                _types = typeLinq.ToArray();
            }

            drawPosition.height = _height;
            var index = EditorGUI.Popup(drawPosition, -1, _keys);
            drawPosition.height = position.height - drawPosition.height - 5;
            drawPosition.y += _height + 5;
            var currentType = string.IsNullOrWhiteSpace(property.managedReferenceFullTypename)
                ? "None"
                : property.managedReferenceFullTypename; 
            EditorGUI.PropertyField(drawPosition, property, new GUIContent($"{label.text} - {property.managedReferenceFullTypename}"), true);
            if (index >= 0)
            {
                if (property.isArray)
                {
                    var newElementIndex = property.arraySize;
                    property.InsertArrayElementAtIndex(newElementIndex);
                    var newProperty = property.GetArrayElementAtIndex(newElementIndex);
                    newProperty.managedReferenceValue = Activator.CreateInstance(_types[index]);
                }
                else
                {
                    property.managedReferenceValue = Activator.CreateInstance(_types[index]);
                }

                property.serializedObject.ApplyModifiedProperties();
            }
        }
        
    }

    public static class TypeExtensions
    {
        public static bool CheckSelfOrBaseType(this Type currentType, Type baseType)
        {
            if (currentType == baseType)
            {
                return true;
            }
            else
            {
                return currentType.CheckBaseType(baseType);
            }
        }
        
        public static bool CheckBaseType(this Type currentType, Type baseType)
        {
            while (currentType.BaseType.IsNotNull())
            {
                currentType = currentType.BaseType;
                if (currentType == baseType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}