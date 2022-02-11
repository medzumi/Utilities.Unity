using UnityEditor;
using UnityEngine;

namespace ApplicationScripts.Ecs.Editor
{
    [CustomPropertyDrawer(typeof(object), true)]
    public class ObjectDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                EditorGUI.PropertyField(position, property,
                    new GUIContent(label.text + $"[{property.type}]"), true);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}