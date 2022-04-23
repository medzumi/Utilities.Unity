using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Utilities.StringLibrary.Editor
{
    [CustomPropertyDrawer(typeof(StringLibraryAttribute))]
    public class StringLibraryDrawer : PropertyDrawer
    {
        private static List<string> _buffer = new List<string>();

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

            var result =
                _strArray[
                    EditorGUI.Popup(position, index, _strArray)];
            var regex = (attribute as StringLibraryAttribute).RegexReplace;
            var replace = (attribute as StringLibraryAttribute).Replace;
            property.stringValue = Regex.Replace(result, regex, replace);
        }
    }
}