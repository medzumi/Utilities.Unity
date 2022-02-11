using System;
using System.Collections.Generic;
using UnityEditor;

namespace Leopotam.EcsLite
{
    public class DebugSystem : IEcsSystem, IEcsPreInitSystem
    {
        private static DebugSystem _instance;
        private HashSet<EcsSystems> _ecsSystems = new HashSet<EcsSystems>();

        public IEnumerable<EcsSystems> EcsSystems => _ecsSystems;
        
        public static DebugSystem Create()
        {
            if (_instance == null)
            {
                _instance = new DebugSystem();
            }

            return _instance;
        }

        public void PreInit(EcsSystems systems)
        {
            _ecsSystems.Add(systems);
        }

        private DebugSystem()
        {
            
        }
    }

    public class DebugWindow : EditorWindow
    {
        private DebugSystem _debugSystem;

        private readonly List<EcsSystemDrawer> _systemDrawers = new List<EcsSystemDrawer>();
        
        [MenuItem("ECS/Debug Window")]
        private static void CreateDebugWindow()
        {
            CreateWindow<DebugWindow>();
        }

        private void OnEnable()
        {
            _debugSystem = DebugSystem.Create();
        }

        public void OnInspectorUpdate()
        {
            DrawSystems(_debugSystem.EcsSystems);
        }

        private void DrawSystems(IEnumerable<EcsSystems> systemsEnumerable)
        {
            int i = 0;
            foreach (var ecsSystems in systemsEnumerable)
            {
                
            }
        }

        private EcsSystemDrawer GetDrawer(int index)
        {
            for (int i = _systemDrawers.Count; i <= index; i++)
            {
                _systemDrawers.Add(new EcsSystemDrawer());
            }

            return _systemDrawers[index];
        }
    }

    public class EcsWorldDrawer : IDisposable
    {
        public void OnGUILayout(string name)
        {
            
        }
        
        public void Dispose()
        {
        }
    }
    
    public class EcsSystemDrawer : IDisposable
    {
        public EcsSystems EcsSystems;
        private bool IsExpanded = false;
        private Dictionary<string, EcsWorldDrawer> _worldDrawers = new Dictionary<string, EcsWorldDrawer>();

        public void OnGUILayout(string name)
        {
            IsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsExpanded, name);
            if (IsExpanded)
            {
                foreach (var keyValuePair in _worldDrawers)
                {
                    if (!_worldDrawers.TryGetValue(keyValuePair.Key, out var drawer))
                    {
                        drawer = new EcsWorldDrawer();
                        _worldDrawers[keyValuePair.Key] = drawer;
                    }
                    drawer.OnGUILayout(keyValuePair.Key);
                }   
            }
        }

        public void Dispose()
        {
        }
    }
}