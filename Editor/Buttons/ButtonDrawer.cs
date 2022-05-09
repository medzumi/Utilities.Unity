using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Utilities.Unity.Buttons;
using Object = UnityEngine.Object;

namespace Utilities.Unity.Editor.Buttons
{
    [CustomEditor(typeof(Object), true)]
    public class ButtonDrawer : UnityEditor.Editor
    {
        private UnityEditor.Editor _editor;
        private List<(string, MethodInfo)> _tuples = new List<(string, MethodInfo)>();

        protected void OnEnable()
        {
            var type = target.GetType();
            foreach (var methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var attribute = methodInfo.GetCustomAttribute<ButtonAttribute>();
                if (attribute != null && methodInfo.GetParameters().Length == 0)
                {
                    if (string.IsNullOrWhiteSpace(attribute.Name))
                    {
                        _tuples.Add((methodInfo.Name, methodInfo));
                    }
                    else
                    {
                        _tuples.Add((attribute.Name, methodInfo));
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginVertical();
            foreach (var tuple in _tuples)
            {
                if (GUILayout.Button(tuple.Item1))
                {
                    tuple.Item2.Invoke(target, null);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}