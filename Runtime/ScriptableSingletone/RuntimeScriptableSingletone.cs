using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Utilities.Unity.ScriptableSingletone
{
    public class RuntimeScriptableSingletone<T> : ScriptableObject where T : RuntimeScriptableSingletone<T>
    {
        private static T _instance;
        private static bool _isLoadingAlready = false;

        protected virtual void Awake()
        {
            
        }

        public static T GetInstance()
        {
            return _instance ? _instance : (_instance = LoadSingletone());
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
                return LoadOrCreateSingletone((RuntimeScriptableSingletoneSettings.GetInstance()).DefaultPath,
                    ( RuntimeScriptableSingletoneSettings.GetInstance()).DefaultResourcesFolderPath);
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

        public static async Task<T> GetInstanceAsync()
        {
            return _instance ? _instance : (_instance = await LoadSingletoneAsync());
        }
        
        private static async Task<T> LoadSingletoneAsync()
        {
            if (_isLoadingAlready) 
            {
                _isLoadingAlready = false;
                return await LoadOrCreateSingletoneAsync(RuntimeScriptableSingletoneSettings.DEFAULT_PATH,
                    RuntimeScriptableSingletoneSettings.DEFAULT_RESOURCES_FOLDER_PATH);
            }

            _isLoadingAlready = true;
            if (typeof(T).GetCustomAttribute(typeof(CustomScriptablePath)) is CustomScriptablePath scriptablePath)
            {
                return await LoadOrCreateSingletoneAsync(scriptablePath.Path, scriptablePath.ResourceFolderPath);
            }
            else
            {
                return await LoadOrCreateSingletoneAsync((await RuntimeScriptableSingletoneSettings.GetInstanceAsync()).DefaultPath,
                    (await RuntimeScriptableSingletoneSettings.GetInstanceAsync()).DefaultResourcesFolderPath);
            }
            return null;
        }

        private static async Task<T> LoadOrCreateSingletoneAsync(string path, string resourceFolderPath)
        {
            T singletone = null;
            try
            {
                var request = Resources.LoadAsync<T>(path);
                while (!request.isDone)
                {
                    await Task.Yield();
                }

                singletone = request.asset as T;
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