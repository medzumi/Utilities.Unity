using System;
using System.Collections.Generic;
using medzumi.Utilities.CodeExtensions;
using Utilities.Unity.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Utilities.Unity.TypeReference.Editor
{
    public interface ITypeReferenceDrawer
    {
        float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent guiContent);

        void CustomGUI(Rect position, SerializedProperty serializedProperty, GUIContent guiContent,
            TypeConstraints typeConstraints);
    }

    [CustomPropertyDrawer(typeof(TypeConstraints), true)]
    public class TypeReferencePropertyDrawer : PropertyDrawer  
    {
        //Because can't create instance without failure when change default sprite for ScriptableObject implementation
        public static Dictionary<SerializedPropertyType, ITypeReferenceDrawer>
            SerializedPropertyTypeTypeReferenceDrawers = new Dictionary<SerializedPropertyType, ITypeReferenceDrawer>();

        private float _height = EditorGUI.GetPropertyHeight(SerializedPropertyType.String, GUIContent.none);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (SerializedPropertyTypeTypeReferenceDrawers.TryGetValue(property.propertyType, out var drawer))
            {
                return drawer.GetPropertyHeight(property, label);
            }

            return EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (SerializedPropertyTypeTypeReferenceDrawers.TryGetValue(property.propertyType, out var drawer))
            {
                drawer.CustomGUI(position, property, label, attribute as TypeConstraints);
            }
            else
            {
                EditorGUI.LabelField(position, "NOT IMPLEMENTED");
            }
        }
    }
}