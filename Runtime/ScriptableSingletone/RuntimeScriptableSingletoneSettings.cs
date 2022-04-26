using System;
using UnityEngine;

namespace Packages.Utilities.Unity.Runtime.ScriptableSingletone
{
    public class RuntimeScriptableSingletoneSettings : RuntimeScriptableSingletone<RuntimeScriptableSingletoneSettings>
    {
        public const string DEFAULT_PATH = "ScriptableSingletones";
        public const string DEFAULT_RESOURCES_FOLDER_PATH = "";

        [SerializeField] private string _defaultPath = DEFAULT_PATH;
        [SerializeField] private string _defaultResourcesFolderPath = DEFAULT_RESOURCES_FOLDER_PATH;

        public string DefaultPath => _defaultPath;
        public string DefaultResourcesFolderPath => _defaultResourcesFolderPath;
    }
}