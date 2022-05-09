using System.Collections.Generic;
using UnityEditor;
using Utilities.Unity.ScriptableSingletone;

namespace Utilities.Unity.Editor.ScriptableSingletone
{
    public class ScriptableSingletoneSettingsProvider : SettingsProvider
    {
        private UnityEditor.Editor _editor;
        private SerializedObject _serializedObject;
        private SerializedProperty _defaultPathProperty;
        private SerializedProperty _defaultResourcesFolderPathProperty;
        
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            
            var provider = new ScriptableSingletoneSettingsProvider("Project/ScriptableSingletoneSettings", SettingsScope.Project);
            return provider;
        }

        public ScriptableSingletoneSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
            _editor = UnityEditor.Editor.CreateEditor(RuntimeScriptableSingletoneSettings.instance);
        }

        public override void OnGUI(string searchContext)
        {
            _editor.OnInspectorGUI();
        }
    }
}