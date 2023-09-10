using System;
using System.Collections.Concurrent;
using UnityEditor;
using UnityEngine;

namespace medzumi.utilities.unity
{
    public class UpdateManager : MonoBehaviour
    {
        private static readonly ConcurrentQueue<Action> _editorActions = new ConcurrentQueue<Action>();
        private static readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();
        private static UpdateManager _updateManager = null;
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.update += EditorUpdate;
        }
#endif
        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitialize()
        {
            _updateManager = (new GameObject("UpdateManager")).AddComponent<UpdateManager>();
            _updateManager.gameObject.hideFlags = HideFlags.HideInHierarchy;
            DontDestroyOnLoad(_updateManager.gameObject);
        }

        public static void SingleActionInEditor(Action action)
        {
#if UNITY_EDITOR
            _editorActions.Enqueue(action);
#endif
        }

        private static void EditorUpdate()
        {
            while (_editorActions.TryDequeue(out var act))
            {
                act?.Invoke();
            }
        }

        private void Update()
        {
            while (_actions.TryDequeue(out var act))
            {
                act?.Invoke();
            }
        }

        public static void SingleAction(Action action)
        {
            _actions.Enqueue(action);
        }
    }
}