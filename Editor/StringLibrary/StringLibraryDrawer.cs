using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Utilities.Unity.StringLibrary;

namespace Utilities.Unity.Editor.StringLibrary
{
    [CustomPropertyDrawer(typeof(StringLibraryAttribute))]
    public class StringLibraryDrawer : PropertyDrawer
    {
        private static List<string> _buffer = new List<string>();
        private static GenericMenu _genericMenu;

        private static GenericMenu GenericMenu
        {
            get
            {
                if (_genericMenu == null || _genericMenu.GetItemCount() + 1 != StringLibrary.instance.Strings.Count)
                {
                    _genericMenu = new GenericMenu();
                    foreach (var VARIABLE in StringLibrary.instance.Strings)
                    {
                        _genericMenu.AddItem(new GUIContent(VARIABLE), false, () =>
                        {
                            var regex = (_activeAttribute as StringLibraryAttribute).RegexReplace;
                            var replace = (_activeAttribute as StringLibraryAttribute).Replace;
                            _activeProperty.stringValue = Regex.Replace(VARIABLE, regex, replace);
                            _activeProperty.serializedObject.ApplyModifiedProperties();
                            _activeProperty = null;
                        });
                    }
                    _genericMenu.AddSeparator("");
                    _genericMenu.AddItem(new GUIContent("Add new"), false, () =>
                    {
                        _editing = true;
                    });
                }

                return _genericMenu;
            }
        }
        private static SerializedProperty _activeProperty;
        private static PropertyAttribute _activeAttribute;
        private static bool _editing;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _buffer.Clear();
            string[] _strArray;
            var index = StringLibrary.instance.Strings.IndexOf(property.stringValue);
            if (index < 0)
            {
                _buffer.AddRange(StringLibrary.instance.Strings);
                _buffer.Add(property.stringValue);
                index = _buffer.Count - 1;
                _strArray = _buffer.ToArray();
            }
            else
            {
                _strArray = StringLibrary.instance.Strings.ToArray();
            }

            position = EditorGUI.PrefixLabel(position, label);
            if (_editing)
            {
                var value = EditorGUI.DelayedTextField(position, property.stringValue);
                if (value != property.stringValue)
                {
                    if (!StringLibrary.instance.Strings.Contains(value))
                    {
                        StringLibrary.instance.Strings.Add(value);
                        EditorUtility.SetDirty(StringLibrary.instance);
                        StringLibrary.instance.Save();
                    }
                    
                    property.stringValue = value;
                    property.serializedObject.ApplyModifiedProperties();
                    _editing = false;
                }
            }
            else if(GUI.Button(position, property.stringValue))
            {
                _activeAttribute = attribute;
                _activeProperty = property;
                GenericMenu.ShowAsContext();
            }
        }
    }
}