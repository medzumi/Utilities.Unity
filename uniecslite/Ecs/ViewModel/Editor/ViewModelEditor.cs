using System;
using System.Linq;
using ApplicationScripts.Properties;
using ApplicationScripts.ViewModel.Events;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ApplicationScripts.ViewModel.Editor
{
    [CustomEditor(typeof(global::ApplicationScripts.ViewModel.ViewModel))]
    public class ViewModelEditor : UnityEditor.Editor
    {
        private static readonly Type[] _types;
        private static readonly GUIContent[] _keys;
        private ReorderableList _reorderableList;
        private SerializedProperty _propertyList;

        static ViewModelEditor()
        {
            _types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => (t.IsSubclassOfGenericTypeDefinition(typeof(ReactiveProperty<>)) && t != typeof(ReactiveProperty<>))
                || (t.IsSubclassOfGenericTypeDefinition(typeof(Event<>)) && t!=typeof(Event<>)))
                .ToArray();
            _keys = _types.Select(t => new GUIContent(t.Name)).ToArray();
        }
        
        public override void OnInspectorGUI()
        {
            if (_reorderableList == null)
            {
                _propertyList = serializedObject.FindProperty("_propertiesList");
                _reorderableList = new ReorderableList(serializedObject, _propertyList, true, true, true, true);
                _reorderableList.onAddDropdownCallback = ONAddCallbackHandler;
                _reorderableList.elementHeightCallback = ElementHeightCallback;
                _reorderableList.drawElementCallback = DrawElementCallback;
            }
            
            _reorderableList.DoLayoutList();
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            var prop = _propertyList.GetArrayElementAtIndex(index);
            var keyProperty = prop.FindPropertyRelative("key");
            var objectProperty = prop.FindPropertyRelative("property");
            var split = objectProperty.managedReferenceFullTypename.Split('.');
            var guiName = $"{split[split.Length - 1]} : {keyProperty.stringValue}";
            EditorGUI.PropertyField(new Rect(rect.x + 40f, rect.y, rect.width - 40f, rect.height), prop, new GUIContent(guiName), prop.isExpanded);
            serializedObject.ApplyModifiedProperties();
        }

        private float ElementHeightCallback(int index)
        {
            var prop = _propertyList.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(prop);
        }

        private void ONAddCallbackHandler(Rect rect, ReorderableList list)
        {
            EditorUtility.DisplayCustomMenu(rect, _keys, -1, CallbackHandler, null);
        }

        private void CallbackHandler(object userdata, string[] options, int selected)
        {
            var count = _propertyList.arraySize;
            _propertyList.InsertArrayElementAtIndex(count);
            _propertyList.GetArrayElementAtIndex(count).managedReferenceValue =
                new global::ApplicationScripts.ViewModel.ViewModel.Pair()
                    {key = $"Element {count}", property = Activator.CreateInstance(_types[selected])};
            serializedObject.ApplyModifiedProperties();
        }
    }
}