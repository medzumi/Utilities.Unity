using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utilities.Unity.Editor
{
    public static class PropertyDrawerExtensions
    {
        public static Rect DrawButton(this Rect rect, GUIContent guiContent, float height, Action action, GUIStyle style = null)
        {
            var bRect = rect;
            bRect.height = height;
            if (GUI.Button(bRect, guiContent,  style ?? GUI.skin.button))
            {
                action?.Invoke();
            }
            rect.height -= height;
            rect.y += height;
            return rect;
        }
        
        public static Rect DrawButton(this Rect rect, GUIContent guiContent, Action action, GUIStyle style = null)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, guiContent);
            var bRect = rect;
            bRect.height = height;
            if (GUI.Button(bRect, guiContent,  style ?? GUI.skin.button))
            {
                action?.Invoke();
            }
            rect.height -= height;
            rect.y += height;
            return rect;
        }

        public static Rect DrawFoldout(this Rect rect, bool state, Action<bool> onChanged)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Boolean, GUIContent.none);
            var tRect = rect;
            tRect.height = height;
            if (state != EditorGUI.Foldout(tRect, state, GUIContent.none))
            {
                onChanged.Invoke(!state);
            }

            rect.height -= height;
            rect.y += height;
            rect.width -= height;

            return rect;
        }
        
        public static Rect DrawButton(this Rect rect, GUIContent guiContent, Action action, Color color)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, guiContent);
            var bRect = rect;
            bRect.height = height;
            (color, GUI.backgroundColor) = (GUI.backgroundColor, color);
            if (GUI.Button(bRect, guiContent))
            {
                (color, GUI.backgroundColor) = (GUI.backgroundColor, color);
                action?.Invoke();
            }
            else
            {
                (color, GUI.backgroundColor) = (GUI.backgroundColor, color);
            }
            rect.height -= height;
            rect.y += height;
            return rect;
        }
        
        public static Rect DrawButton(this Rect rect, string label, Action action, GUIStyle style = null)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, new GUIContent(label));
            var bRect = rect;
            bRect.height = height;
            if (GUI.Button(bRect, new GUIContent(label), style ?? GUI.skin.button))
            {
                action?.Invoke();
            }
            rect.height -= height;
            rect.y += height;
            return rect;
        }
        
        
        public static Rect DrawButton(this Rect rect, string label, Action action, Color color)
        {
            var guiContent = new GUIContent(label);
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, guiContent);
            var bRect = rect;
            bRect.height = height;
            (color, GUI.backgroundColor) = (GUI.backgroundColor, color);
            if (GUI.Button(bRect, guiContent))
            {
                (color, GUI.backgroundColor) = (GUI.backgroundColor, color);
                action?.Invoke();
            }
            else
            {
                (color, GUI.backgroundColor) = (GUI.backgroundColor, color);
            }
            rect.height -= height;
            rect.y += height;
            return rect;
        }

        public static Rect DrawSplitHorizontal(this Rect rect, int count, IEnumerable<Func<Rect, Rect>> actions)
        {
            int index = 0;
            rect.width = rect.width / count;
            var height = 0f;
            foreach (Func<Rect, Rect> action in actions)
            {
                if (index >= count)
                {
                    break;
                }

                var nRect = action.Invoke(rect);
                rect.x += rect.width;
                height = Mathf.Max(height,  rect.height - nRect.height);
            }

            rect.height = height;
            rect.y += height;
            return rect;
        }
        
        public static Rect DrawSplitHorizontal(this Rect rect, IEnumerable<Func<Rect, Rect>> actions)
        {
            var height = 0f;
            var hRect = rect;
            foreach (Func<Rect, Rect> action in actions)
            {
                var newRect = action.Invoke(hRect);
                hRect.x += newRect.x;
                hRect.width = newRect.width;
                height = Mathf.Max(height, rect.height - newRect.height);
            }

            rect.height = height;
            rect.y += height;
            return rect;
        }

        public static Rect DrawPropertyField(this Rect rect, SerializedProperty property, GUIContent label)
        {
            if (property.hasChildren)
            {
                var height = EditorGUI.GetPropertyHeight(property, label, property.hasVisibleChildren);
                var currentRect = new Rect(rect.position, new Vector2(rect.width, height));
                EditorGUI.PropertyField(currentRect, property, label, property.hasVisibleChildren);
                return new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
            }
            else
            {
                return rect;
            }
        }
        
        public static Rect DrawPropertyField(this Rect rect, SerializedProperty property)
        {
            if (property.hasChildren)
            {
                var height = EditorGUI.GetPropertyHeight(property);
                var currentRect = new Rect(rect.position, new Vector2(rect.width, height));
                EditorGUI.PropertyField(currentRect, property, property.hasVisibleChildren);
                return new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
            }

            return rect;
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

        public static Rect DrawLabel(this Rect rect, GUIContent guiContent)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.String, guiContent);
            var pos = rect;
            pos.height = height;
            EditorGUI.LabelField(pos, guiContent);
            rect.y += height;
            rect.height -= height;
            return rect;
        }

        public static float GetPropertyHeight(this SerializedProperty property, GUIContent guiContent)
        {
            if (property.hasChildren)
            {
                return EditorGUI.GetPropertyHeight(property, guiContent, property.hasVisibleChildren);
            }

            return 0f;
        }

        public static Rect DrawPropertyField(this Rect rect, SerializedProperty serializedProperty, string serializedPropertyDisplayName)
        {
            if (serializedProperty.hasChildren)
            {
                var height = EditorGUI.GetPropertyHeight(serializedProperty);
                var currentRect = new Rect(rect.position, new Vector2(rect.width, height));
                EditorGUI.PropertyField(currentRect, serializedProperty, new GUIContent(serializedPropertyDisplayName), serializedProperty.hasVisibleChildren);
                return new Rect(rect.x, rect.y + height, rect.width, rect.height - height);
            }

            return rect;
        }
    }
}