using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationScripts.Properties;
using ApplicationScripts.ViewModel.Binds;
using ApplicationScripts.ViewModel.Events;
using UnityEditor;
using UnityEngine;

namespace ApplicationScripts.ViewModel.Editor
{
    [CustomPropertyDrawer(typeof(SelectBindAttribute))]
    public class BindPropertyDrawer : PropertyDrawer
    {
        private Type[] _types;
        private GUIContent[] _keys;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return true;
        }

        public static Type GetTypeByName(string typeName)
        {
            if(string.IsNullOrEmpty(typeName))
                return null;

            var typeSplit = typeName.Split(char.Parse(" "));
            var typeAssembly = typeSplit[0];
            var typeClass = typeSplit[1];

            return Type.GetType(typeClass + ", " + typeAssembly);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_keys == null)
            {
                Type propType = null;
                if (!string.IsNullOrEmpty(property.managedReferenceFieldTypename))
                {
                    propType = GetTypeByName(property.managedReferenceFieldTypename);
                }
                _types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(t => t.IsSubclassDeep(typeof(Bind)) && !t.IsAbstract && !t.IsGenericType 
                    && (propType == null || t.IsSubclassOf(propType)))
                    .ToArray();
                _keys = _types.Select(t => new GUIContent(t.Name)).ToArray();
            }

            var width = 0f;
            var split = property.managedReferenceFullTypename.Split('.');
            var guiContent = new GUIContent(split[split.Length-1]);
            if (_types.Length > 1)
            {
                width = GUI.skin.button.CalcSize(guiContent).x + 10f;
            }
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - width - 5f, position.height), property, label, property.isExpanded);
            if (_types.Length > 1)
            {
                if (EditorGUI.DropdownButton(new Rect(position.x + position.width - width, position.y, width, 25f),
                    guiContent, FocusType.Keyboard))
                {
                    var rect = new Rect(UnityEngine.Event.current.mousePosition, Vector2.zero);
                    EditorUtility.DisplayCustomMenu(rect, _keys, -1, (data, options, selected) =>
                    {
                        property.managedReferenceValue = Activator.CreateInstance(_types[selected]);
                        property.serializedObject.ApplyModifiedProperties();
                    }, null);
                }
            }
        }
    }

    [CustomPropertyDrawer(typeof(BindStringAttribute))]
    public class BindPropertyAttributeDrawer : PropertyDrawer
    {
        private static FieldInfo _fieldInfo;

        private static PropertyInfo _propertyInfo;
    
        static BindPropertyAttributeDrawer()
        {
            _fieldInfo =
                typeof(global::ApplicationScripts.ViewModel.ViewModel).GetField("_propertiesList", BindingFlags.Instance | BindingFlags.NonPublic);

            _propertyInfo = typeof(Bind).GetProperty("BindType",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
    
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property = property.Copy();
            var width = GUI.skin.label.CalcSize(label).x*2;
            EditorGUI.LabelField(new Rect(position.position, new Vector2(width, position.height)), label);
            if (GUI.Button(new Rect(position.x + width, position.y, position.width - width, position.height), property.stringValue))
            {
                var menu = new GenericMenu();
                var parentProperty = property.GetParentProperty();
                var viewModelProperty = parentProperty.FindPropertyRelative("_viewModel");
                var pairs = (List<global::ApplicationScripts.ViewModel.ViewModel.Pair>)_fieldInfo.GetValue(viewModelProperty.objectReferenceValue as global::ApplicationScripts.ViewModel.ViewModel);

                var splited = parentProperty.managedReferenceFullTypename.Split(' ');

                var type = Type.GetType($"{splited[1]}, {splited[0]}");

                if (type.TryGetGenericTypeOfDefinition(typeof(PropertyBind<>), out var bindType))
                {
                    var parameterType = bindType.GetGenericArguments()[0];
                    var reactiveType = typeof(ReactiveProperty<>).MakeGenericType(parameterType);
                    var ids = pairs.Where(pair => pair.property.GetType().IsSubclassDeep(reactiveType))
                        .Select(pair => pair.key);

                    foreach (var s in ids)
                    {
                        menu.AddItem(new GUIContent(s), false, () =>
                        {
                            property.stringValue = s;
                            property.serializedObject.ApplyModifiedProperties();
                        });
                    }
                    
                    menu.ShowAsContext();
                }
                if (type.TryGetGenericTypeOfDefinition(typeof(EventBind<>), out var eventBindType))
                {
                    var parameterType = eventBindType.GetGenericArguments()[0];
                    var reactiveType = typeof(Event<>).MakeGenericType(parameterType);
                    var ids = pairs.Where(pair => pair.property.GetType().IsSubclassDeep(reactiveType))
                        .Select(pair => pair.key);

                    foreach (var s in ids)
                    {
                        menu.AddItem(new GUIContent(s), false, () =>
                        {
                            property.stringValue = s;
                            property.serializedObject.ApplyModifiedProperties();
                        });
                    }
                    
                    menu.ShowAsContext();
                }
            }
        }
    }

    public static class PropertyExtensions
    {
        public static SerializedProperty GetParentProperty(this SerializedProperty property)
        {
            var path = property.propertyPath;
            var split = path.Split('.');
            var count = split.Length - 2;
            property = property.serializedObject.FindProperty(split[0]);
            for (int i = 0; i < count; i++)
            {
                property = property.FindPropertyRelative(split[i + 1]);
            }

            return property;
        }
    }
}