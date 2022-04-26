using System;
using System.Reflection;
using UnityEngine;

namespace Packages.Utilities.Unity.Runtime.ScriptableSingletone
{
    public class RuntimeScriptableSingletone<T> : ScriptableObject where T : RuntimeScriptableSingletone<T>
    {
        private static T _instance;
        private static bool _isLoadingAlready = false;

        public static T instance
        {
            get
            {
                return _instance ? _instance : (_instance = LoadSingletone());
            }
        }
        
        private static T LoadSingletone()
        {
            if (_isLoadingAlready) 
            {
                _isLoadingAlready = false;
                return LoadOrCreateSingletone(RuntimeScriptableSingletoneSettings.DEFAULT_PATH,
                    RuntimeScriptableSingletoneSettings.DEFAULT_RESOURCES_FOLDER_PATH);
            }

            _isLoadingAlready = true;
            if (typeof(T).GetCustomAttribute(typeof(CustomScriptablePath)) is CustomScriptablePath scriptablePath)
            {
                return LoadOrCreateSingletone(scriptablePath.Path, scriptablePath.ResourceFolderPath);
            }
            else
            {
                return LoadOrCreateSingletone(RuntimeScriptableSingletoneSettings.instance.DefaultPath,
                    RuntimeScriptableSingletoneSettings.instance.DefaultResourcesFolderPath);
            }
            return null;
        }

        private static T LoadOrCreateSingletone(string path, string resourceFolderPath)
        {
            T singletone = null;
            try
            {
                singletone = Resources.Load<T>(path);
                if (singletone)
                {
                    return singletone;
                }
                else
                {
#if UNITY_EDITOR
                    var availableSingletones = Resources.LoadAll<T>(string.Empty);
                    if (availableSingletones.Length > 0)
                    {
                        if (availableSingletones[0] is RuntimeScriptableSingletoneSettings runtimeScriptableSingletone)
                        {
                            path = runtimeScriptableSingletone.DefaultPath;
                            resourceFolderPath = runtimeScriptableSingletone.DefaultResourcesFolderPath;
                        }
                        var currentPath = UnityEditor.AssetDatabase.GetAssetPath(availableSingletones[0]);
                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Application.dataPath, resourceFolderPath,
                            "Resources", path));
                        UnityEditor.AssetDatabase.SaveAssets();
                        UnityEditor.AssetDatabase.Refresh();
                        var toPath = System.IO.Path.Combine("Assets", resourceFolderPath, "Resources", path,
                            $"{typeof(T).Name}.asset");
                        UnityEditor.AssetDatabase.MoveAsset(currentPath, toPath);
                        for (int i = 1; i < availableSingletones.Length; i++)
                        {
                            UnityEditor.AssetDatabase.DeleteAsset(
                                UnityEditor.AssetDatabase.GetAssetPath(availableSingletones[i]));
                        }

                        UnityEditor.AssetDatabase.SaveAssets();
                        return availableSingletones[0];
                    }
                    else
                    {
                        singletone = CreateInstance<T>();
                        string directoryPath =
                            System.IO.Path.Combine(Application.dataPath, resourceFolderPath, "Resources", path); 
                        System.IO.Directory.CreateDirectory(directoryPath);
                        UnityEditor.AssetDatabase.SaveAssets();
                        UnityEditor.AssetDatabase.Refresh();
                        UnityEditor.AssetDatabase.CreateAsset(singletone,
                            System.IO.Path.Combine("Assets", resourceFolderPath, "Resources", path, $"{typeof(T).Name}.asset"));
                        UnityEditor.AssetDatabase.SaveAssets();
                        return singletone;
                    }
#endif
                    return CreateInstance<T>();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _isLoadingAlready = false;
                return CreateInstance<T>();
            }
        }
    }

    public class CustomScriptablePath : Attribute
    {
        public string Path;
        public string ResourceFolderPath;
    }
}