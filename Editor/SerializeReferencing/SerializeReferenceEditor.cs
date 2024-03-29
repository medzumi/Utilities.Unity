﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using medzumi.Utilities.CodeExtensions;
using UnityEditor;
using UnityEngine;
using Utilities.Unity.TypeReference;
using Object = UnityEngine.Object;

namespace Utilities.Unity.Editor.SerializeReferencing
{
    [CustomPropertyDrawer(typeof(SerializeReferenceConstraintsAttribute), true)]
    [CustomPropertyDrawer(typeof(SerializeReference))]
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
            return EditorGUI.GetPropertyHeight(property, label, true) +
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
                var split = property.managedReferenceFieldTypename.Split(' ');
                var qualifiedName = Assembly.CreateQualifiedName(split[0], split[1]);
                var baseType = Type.GetType(qualifiedName);
                IEnumerable<Type> typeLinq = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes());
                var uObjType = typeof(Object);
                if (attribute is BaseConstraintsAttribute attr)
                {
                    typeLinq = typeLinq.Where(type => attr.IsMatched(type) 
                                                      && baseType.IsAssignableFrom(type) 
                                                      && !uObjType.IsAssignableFrom(type));
                }
                else
                {
                    typeLinq = typeLinq.Where(type => baseType.IsAssignableFrom(type) && !uObjType.IsAssignableFrom(type));
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