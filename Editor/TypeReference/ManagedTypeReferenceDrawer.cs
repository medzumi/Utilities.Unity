using System;
using medzumi.Utilities.GenericPatterns;
using medzumi.utilities.unity;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Unity.Editor;
using Object = UnityEngine.Object;

namespace Utilities.Unity.TypeReference.Editor
{
    public class ManagedTypeReferenceDrawer : ScriptableObject, ITypeReferenceDrawer, ITypeSetter
    {
        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            EditorApplication.delayCall += () =>
            {
                TypeReferencePropertyDrawer.SerializedPropertyTypeTypeReferenceDrawers[
                        SerializedPropertyType.ManagedReference] =
                    ObjectFactory.CreateInstance<ManagedTypeReferenceDrawer>();
            };
        }

        private TypeReferenceSearchWindowProvider _typeReferenceSearchWindowProvider;

        private void Awake()
        {
            _typeReferenceSearchWindowProvider = ObjectFactory.CreateInstance<TypeReferenceSearchWindowProvider>();
            _trashContent = new GUIContent(_trashTexture, "Delete");
            _height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, GUIContent.none) * 1.5f;
            _guiContent = new GUIContent("Create Instance");
        }

        [SerializeField] private Texture2D _trashTexture;
        private GUIContent _trashContent;
        private GUIContent _guiContent;

        private float _height;

        public float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent guiContent)
        {
            return serializedProperty.managedReferenceValue != null
                ? Mathf.Max(EditorGUI.GetPropertyHeight(serializedProperty, guiContent), _height)
                : EditorGUI.GetPropertyHeight(SerializedPropertyType.String, guiContent);
        }

        public void CustomGUI(Rect position, SerializedProperty serializedProperty, GUIContent guiContent,
            TypeConstraints typeConstraints)
        {
            if (serializedProperty.managedReferenceId != -2)
            {
                position.width -= _height;
                position.DrawPropertyField(serializedProperty,
                    new GUIContent($"{serializedProperty.displayName} : {serializedProperty.managedReferenceFullTypename}", serializedProperty.managedReferenceFullTypename));
                position.x += position.width;
                position.width = _height;
                position.DrawButton(_trashContent, position.height, () =>
                {
                    serializedProperty.managedReferenceValue = null;
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                });
            }
            else
            {
                position = EditorGUI.PrefixLabel(position, guiContent);
                position.DrawButton(_guiContent, () =>
                {
                    _typeReferenceSearchWindowProvider.Initialize(serializedProperty, typeConstraints, this, serializedProperty.GetPropertyTypes().FieldType);
                    SearchWindow.Open(
                        new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition),
                            _typeReferenceSearchWindowProvider.MaxWidth), _typeReferenceSearchWindowProvider);
                });
            }
        }

        public void SetType(Type type, SerializedProperty serializedProperty)
        {
            serializedProperty.managedReferenceValue = Activator.CreateInstance(type);
            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}