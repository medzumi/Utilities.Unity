using System;
using UnityEditor;
using UnityEngine;
using Utilities.Unity.PropertyAttributes;

namespace Utilities.Unity.Editor
{
    [CustomPropertyDrawer(typeof(AutoBindAttribute))]
    public class AutoBindAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorGUI.PropertyField(position, property, label))
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (property.serializedObject.targetObject is Component component && !property.objectReferenceValue)
                {
                    var type = property.type
                                       .Replace("PPtr<$", String.Empty)
                                       .TrimEnd('>');
                    if (property.objectReferenceValue = (attribute as AutoBindAttribute).AutoBind(property.serializedObject.targetObject, type))
                    {
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }
    }
}