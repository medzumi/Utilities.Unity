using System;
using System.Linq;
using ApplicationScripts.CodeExtensions;
using ApplicationScripts.Properties;
using ApplicationScripts.ViewModel.ReflectionBind;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ApplicationScripts.ViewModel.Editor
{
    [CustomPropertyDrawer(typeof(ReflectionBindStringAttribute))]
    public class ReflectionBindDrawer : PropertyDrawer
    {
        private Type _findableType;
        private Object _current;
        private string[] _keys = new string[0];
        
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var parentProperty = property.GetParentProperty();
            if (_findableType.IsNull())
            {
                _findableType = Type.GetType(parentProperty.FindPropertyRelative("_type").stringValue);
                _findableType = typeof(ReactiveProperty<>).MakeGenericType(_findableType);
            }

            var goProperty = parentProperty.FindPropertyRelative("_object");
            var newGameObject = goProperty.objectReferenceValue;
            if (newGameObject != _current && !newGameObject)
            {
                _keys = new string[0];
            }
            if ((newGameObject != _current) && newGameObject)
            {
                _current = newGameObject;
                var goType = _current.GetType();
                var properties = goType
                    .GetPropertiesOrFields();
                var linqProperties = properties.Where(pf => pf.GetType().IsSubclassDeep(_findableType));
                _keys = linqProperties.Select(p => p.GetName()).ToArray();
            }
            
            var currentIndex = _keys.IndexOf(property.stringValue);
            var index = EditorGUI.Popup(position, currentIndex, _keys);
            if (index != currentIndex)
            {
                property.stringValue = _keys[index];
            }
        }
    }
}