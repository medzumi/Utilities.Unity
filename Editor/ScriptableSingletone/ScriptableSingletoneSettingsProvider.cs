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
            Initialize();
        }

        private async void Initialize()
        {
            _editor = UnityEditor.Editor.CreateEditor(await RuntimeScriptableSingletoneSettings.GetInstanceAsync());
        }

        public override void OnGUI(string searchContext)
        {
            if(_editor)
                _editor.OnInspectorGUI();
        }
    }
}