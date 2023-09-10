using System;
using System.Collections.Generic;
using System.Reflection;
using medzumi.Utilities;
using medzumi.Utilities.CodeExtensions;
using UnityEditor;
using UnityEngine;
using Utilities.Unity.Buttons;
using Object = UnityEngine.Object;

namespace Utilities.Unity.Editor.Buttons
{
    // [CustomEditor(typeof(Object), true)]
    // public class ButtonDrawer : UnityEditor.Editor
    // {
    //     private UnityEditor.Editor _editor;
    //     private Dictionary<string, MethodInfo> _tuples = new Dictionary<string, MethodInfo>();
    //
    //     protected void OnEnable()
    //     {
    //         var type = target.GetType();
    //         while (type != null)
    //         {
    //             foreach (var methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
    //             {
    //                 var attribute = methodInfo.GetCustomAttribute<ButtonAttribute>();
    //                 if (attribute != null && methodInfo.GetParameters().Length == 0)
    //                 {
    //                     var key = string.IsNullOrWhiteSpace(attribute.Name) ? methodInfo.Name : attribute.Name; 
    //                     if (!_tuples.ContainsKey(key))
    //                     {
    //                         _tuples[key] = methodInfo;
    //                     }
    //                 }
    //             }
    //
    //             type = type.BaseType;
    //         }
    //     }
    //
    //     public override void OnInspectorGUI()
    //     {
    //         base.OnInspectorGUI();
    //
    //         EditorGUILayout.BeginVertical();
    //         foreach (var tuple in _tuples)
    //         {
    //             if (GUILayout.Button(tuple.Key))
    //             {
    //                 tuple.Value.Invoke(target, null);
    //             }
    //         }
    //         EditorGUILayout.EndHorizontal();
    //     }
    // }
}