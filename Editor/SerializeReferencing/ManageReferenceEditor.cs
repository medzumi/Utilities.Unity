using System;
using UnityEditor;
using UnityEngine;

namespace Utilities.Unity.Editor.SerializeReferencing
{
    [CustomPropertyDrawer(typeof(object), true)]
    [Obsolete("Doesn't work. It's magic")]
    public class ManageReferenceEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                var guiContentText = string.IsNullOrWhiteSpace(property.managedReferenceFullTypename)
                    ? property.managedReferenceFieldTypename
                    : property.managedReferenceFullTypename;
                guiContentText = $"{label.text} - {guiContentText}";
                var guiContent = new GUIContent(guiContentText, label.image, label.tooltip);
                EditorGUI.PropertyField(position, property, guiContent, property.isExpanded);
            }

            else
            {
                EditorGUI.PropertyField(position, property, label, property.isExpanded);
            }
        }
    }
}