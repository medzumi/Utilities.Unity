using System;
using System.Linq;
using ApplicationScripts.Ecs.Unity;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ApplicationScripts.Ecs.Editor
{
    [CustomEditor(typeof(ConvertToEntity))]
    public class ConvertToEntityEditor : UnityEditor.Editor
    {
        private static Type[] _availableTypes;
        private static GUIContent[] _availableTypeNames;

        static ConvertToEntityEditor()
        {
            var tuples = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsValueType && type.FullName.Contains("ApplicationScripts"))
                .Select(type => (type, type.FullName.Replace('.', '/')));
            _availableTypeNames = tuples.Select(tuple => new GUIContent(tuple.Item2)).ToArray();
            _availableTypes = tuples.Select(tuple => tuple.type).ToArray();
        }
        
        private ReorderableList _reorderableList;
        private SerializedProperty _listProperty;
        
        public override void OnInspectorGUI()
        {
            if (_listProperty == null)
            {
                _listProperty = serializedObject.FindProperty("_list");
                _reorderableList = new ReorderableList(serializedObject, _listProperty);
                _reorderableList.onAddDropdownCallback = ONAddDropdownCallback;
                _reorderableList.elementHeightCallback = ElementHeightCallback;
                _reorderableList.drawElementCallback = DrawElementCallback;
            }

            var enumerator = serializedObject.GetIterator();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("worldName"));
            _reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            rect.x += 10f;
            rect.width -= 10f;
            EditorGUI.PropertyField(rect, _listProperty.GetArrayElementAtIndex(index), true);
        }

        private float ElementHeightCallback(int index)
        {
            return EditorGUI.GetPropertyHeight(_listProperty.GetArrayElementAtIndex(index), true);
        }

        private void ONAddDropdownCallback(Rect buttonrect, ReorderableList list)
        {
            EditorUtility.DisplayCustomMenu(buttonrect, _availableTypeNames, -1, Callback, null);
        }

        private void Callback(object userdata, string[] options, int selected)
        {
            _listProperty.InsertArrayElementAtIndex(_listProperty.arraySize);
            _listProperty.GetArrayElementAtIndex(_listProperty.arraySize - 1).managedReferenceValue = Activator.CreateInstance(_availableTypes[selected]);
            serializedObject.ApplyModifiedProperties();
        }
    }
}