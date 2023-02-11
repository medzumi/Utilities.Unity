using medzumi.Utilities.GenericPatterns.Datas;
using UnityEditor;
using UnityEngine;

namespace Utilities.Unity.Editor
{
    [CustomPropertyDrawer(typeof(ValueTuple<,>))]
    public class ValueTuplePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label) + 
                   EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(ValueTuple<object,object>.Item1))) +
                   EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(ValueTuple<object,object>.Item2)));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var drawablePosition = position;
            drawablePosition.height = EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label);
            EditorGUI.LabelField(drawablePosition, label);
            var childProperty = property.FindPropertyRelative(nameof(ValueTuple<object, object>.Item1));
            drawablePosition.y += drawablePosition.height;
            drawablePosition.height =
                EditorGUI.GetPropertyHeight(childProperty, true);
            EditorGUI.PropertyField(drawablePosition, childProperty, true);
            childProperty = property.FindPropertyRelative(nameof(ValueTuple<object, object>.Item2));
            drawablePosition.y += drawablePosition.height;
            drawablePosition.height = EditorGUI.GetPropertyHeight(childProperty, true);
            EditorGUI.PropertyField(drawablePosition, childProperty, true);
        }
    }

    [CustomPropertyDrawer(typeof(ValueTuple<,,>))]
    public class ValueTuplePropertyDrawer3 : ValueTuplePropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(ValueTuple<object, object,object>.Item3)));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var childProperty = property.FindPropertyRelative(nameof(ValueTuple<object, object, object>.Item3));
            var drawableRect = position;
            drawableRect.height = EditorGUI.GetPropertyHeight(childProperty);
            drawableRect.y = position.y + position.height - drawableRect.height;
            position.height -= drawableRect.height;
            EditorGUI.PropertyField(drawableRect, childProperty, true);
            base.OnGUI(position, property, label);
        }
    }

    [CustomPropertyDrawer(typeof(ValueTuple<,,,>))]
    public class ValueTuplePropertyDrawer4 : ValueTuplePropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(ValueTuple<object, object,object, object>.Item4)));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var childProperty = property.FindPropertyRelative(nameof(ValueTuple<object, object, object, object>.Item4));
            var drawableRect = position;
            drawableRect.height = EditorGUI.GetPropertyHeight(childProperty);
            drawableRect.y = position.y + position.height - drawableRect.height;
            position.height -= drawableRect.height;
            EditorGUI.PropertyField(drawableRect, childProperty, true);
            base.OnGUI(position, property, label);
        }
    }   
}