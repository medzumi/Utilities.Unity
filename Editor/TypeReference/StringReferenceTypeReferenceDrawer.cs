using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Unity.Editor;

namespace Utilities.Unity.TypeReference.Editor
{
    public class StringReferenceTypeReferenceDrawer : ScriptableObject, ITypeReferenceDrawer, ITypeSetter
    {
        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            EditorApplication.delayCall += () =>
            {
                TypeReferencePropertyDrawer.SerializedPropertyTypeTypeReferenceDrawers[SerializedPropertyType.String] =
                    ObjectFactory.CreateInstance<StringReferenceTypeReferenceDrawer>();
            };
        }

        private TypeReferenceSearchWindowProvider _typeReferenceSearchWindowProvider;

        private void Awake()
        {
            _typeReferenceSearchWindowProvider = ObjectFactory.CreateInstance<TypeReferenceSearchWindowProvider>();
        }

        public float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent guiContent)
        {
            return EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, guiContent);
        }

        public void CustomGUI(Rect position, SerializedProperty serializedProperty, GUIContent guiContent,
            TypeConstraints typeConstraints)
        {
            position = EditorGUI.PrefixLabel(position, guiContent);
            position.DrawButton(guiContent, () =>
            {
                _typeReferenceSearchWindowProvider.Initialize(serializedProperty, typeConstraints, this, typeof(object));
                SearchWindow.Open(
                    new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition),
                        _typeReferenceSearchWindowProvider.MaxWidth), _typeReferenceSearchWindowProvider);
            });
        }

        public void SetType(Type type, SerializedProperty serializedProperty)
        {
            serializedProperty.stringValue = type.AssemblyQualifiedName;
            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}