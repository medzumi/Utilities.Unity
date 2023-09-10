using System;
using System.Collections.Generic;
using medzumi.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Unity.Editor;

namespace Utilities.Unity.TypeReference.Editor
{
    public class GenericTypeReferenceDrawer : ScriptableObject, ITypeReferenceDrawer
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.delayCall += () =>
            {
                TypeReferencePropertyDrawer.SerializedPropertyTypeTypeReferenceDrawers[SerializedPropertyType.Generic] =
                    ObjectFactory.CreateInstance<GenericTypeReferenceDrawer>();
            };
        }
        
        public static Dictionary<Type, ITypeReferenceDrawer> TypeReferenceDrawers =
            new Dictionary<Type, ITypeReferenceDrawer>();

        public float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent guiContent)
        {
            var currentFieldInfo = serializedProperty.GetPropertyTypes();
            if (TypeReferenceDrawers.TryFindWithRecursiveType(currentFieldInfo.FieldType, out var drawer))
            {
                return drawer.GetPropertyHeight(serializedProperty, guiContent);
            }

            return EditorGUI.GetPropertyHeight(SerializedPropertyType.String, guiContent);
        }

        public void CustomGUI(Rect position, SerializedProperty serializedProperty, GUIContent guiContent,
            TypeConstraints typeConstraints)
        {
            var currentFieldInfo = serializedProperty.GetPropertyTypes();
            if (TypeReferenceDrawers.TryFindWithRecursiveType(currentFieldInfo.FieldType, out var drawer))
            {
                drawer.CustomGUI(position, serializedProperty, guiContent, typeConstraints);
            }
            else
            {
                position = EditorGUI.PrefixLabel(position, guiContent);
                EditorGUI.LabelField(position, "NOT IMPLEMENTED GUI");
            }
        }
    }
}