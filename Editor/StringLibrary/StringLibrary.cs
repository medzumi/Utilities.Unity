using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Utilities.StringLibrary.Editor
{
    [FilePath("ProjectSettings/StringLibrary.asset", FilePathAttribute.Location.ProjectFolder)]
    internal class StringLibrary : ScriptableSingleton<StringLibrary>
    {
        public List<string> _strings;

        public List<string> Strings => _strings;

        public void Save()
        {
            Save(true);
        }

        public SerializedObject GetSerializedObject()
        {
            return new SerializedObject(this);
        }
    }
    
    class StringLibraryProvider : SettingsProvider
    {
        SerializedObject m_SerializedObject;
        SerializedProperty m_Strings;

        private float _scroll = 0;

        private GUIContent _guiContent;

        public StringLibraryProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords) {}

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            StringLibrary.instance.hideFlags = HideFlags.None;
            StringLibrary.instance.Save();
            m_SerializedObject = StringLibrary.instance.GetSerializedObject();
            m_Strings = m_SerializedObject.FindProperty("_strings");
        }

        public override void OnGUI(string searchContext)
        {
            m_SerializedObject.Update();
            EditorGUI.BeginChangeCheck();
            _scroll = EditorGUILayout.BeginScrollView(new Vector2(0, _scroll)).y;
            EditorGUILayout.PropertyField(m_Strings);
            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck())
            {
                m_SerializedObject.ApplyModifiedProperties();
                StringLibrary.instance.Save();
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateStringLibraryProvider()
        {
            var provider = new StringLibraryProvider("Project/StringLibrary", SettingsScope.Project);
            return provider;
        }
    }
}

