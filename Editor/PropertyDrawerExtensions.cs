using System;
using UnityEditor;
using UnityEngine;

namespace Utilities.Unity.Editor
{
    public static class PropertyDrawerExtensions
    {
        public static Rect DrawPropertyField(this Rect rect, SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(property, label);
            var currentRect = new Rect(rect.position, new Vector2(rect.width, height));
            EditorGUI.PropertyField(currentRect, property, label);
            return new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
        }
        
        public static Rect DrawPropertyField(this Rect rect, SerializedProperty property)
        {
            var height = EditorGUI.GetPropertyHeight(property);
            var currentRect = new Rect(rect.position, new Vector2(rect.width, height));
            EditorGUI.PropertyField(currentRect, property);
            return new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
        }

        public static Rect DrawObjectReferenceField(this Rect rect, SerializedProperty property)
        {
            var height = EditorGUI.GetPropertyHeight(property);
            var currentRect = new Rect(rect.position, new Vector2(rect.width, height));
            EditorGUI.ObjectField(currentRect, property);
            return new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
        }
        
        public static Rect DrawObjectReferenceField(this Rect rect, SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(property);
            var currentRect = new Rect(rect.position, new Vector2(rect.width, height));
            EditorGUI.ObjectField(currentRect, property, label);
            return new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
        }
        
        public static Rect DrawObjectReferenceField(this Rect rect, SerializedProperty property, Type type)
        {
            var height = EditorGUI.GetPropertyHeight(property);
            var currentRect = new Rect(rect.position, new Vector2(rect.width, height));
            EditorGUI.ObjectField(currentRect, property, type);
            return new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
        }
        
        public static Rect DrawObjectReferenceField(this Rect rect, SerializedProperty property, Type type, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(property);
            var currentRect = new Rect(rect.position, new Vector2(rect.width, height));
            EditorGUI.ObjectField(currentRect, property, type, label);
            return new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
        }

        public static Rect DrawPopup(this Rect rect, int selectedIndex, string[] strings, out int result)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, GUIContent.none);
            result = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, height), selectedIndex, strings);
            return new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
        }
        
        public static Rect DrawPopup(this Rect rect, int selectedIndex, GUIContent[] strings, out int result)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, GUIContent.none);
            result = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, height), selectedIndex, strings);
            return new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
        }
        
        public static Rect DrawPopup(this Rect rect, int selectedIndex, GUIContent[] strings, out int result, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, GUIContent.none);
            result = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, height), label, selectedIndex, strings);
            return new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
        }
        
        public static Rect DrawPopup(this Rect rect, int selectedIndex, string[] strings, out int result, string label)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, GUIContent.none);
            result = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, height), label, selectedIndex, strings);
            return new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
        }

        public static Rect DrawButton(this Rect rect, string label, out bool result, float buttonHeight = 0)
        {
            if (Mathf.Approximately(0, buttonHeight))
            {
                buttonHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, GUIContent.none);
            }

            result = GUI.Button(new Rect(rect.x, rect.y, rect.width, buttonHeight), label);
            return new Rect(rect.x, rect.y + buttonHeight, rect.width, rect.height + buttonHeight);
        }

        public static Rect DrawPrefixLabel(this Rect rect, string label, out Rect nextVerticalRect)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.String, GUIContent.none);
            nextVerticalRect = new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
            return EditorGUI.PrefixLabel(rect, new GUIContent(label));
        }
    }
}