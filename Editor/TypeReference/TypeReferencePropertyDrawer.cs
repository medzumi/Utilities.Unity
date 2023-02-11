using System;
using System.Linq;
using medzumi.Utilities.CodeExtensions;
using UnityEditor;
using UnityEngine;


namespace Utilities.Unity.TypeReference.Editor
{
    [CustomPropertyDrawer(typeof(TypeReferenceConstraintsAttribute))]
    [CustomPropertyDrawer(typeof(TypeReference), true)]
    public class TypeReferencePropertyDrawer : PropertyDrawer
    {
        public static TypeReferenceConstraintsAttribute TypeReferenceConstraintsAttribute;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, label);     
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);
            
            var assemblyQualifiedProperty = property.FindPropertyRelative("_assemblyQualifiedName");
            if (GUI.Button(position, Type.GetType(assemblyQualifiedProperty.stringValue)?.FullName))
            {
                if (attribute is TypeReferenceConstraintsAttribute constraintsAttribute)
                {
                    var genericMenu = new GenericMenu();
                    genericMenu.AddItem(new GUIContent("Null"), false, () =>
                    {
                        assemblyQualifiedProperty.stringValue = null;
                    });
                    
                    foreach (var type in constraintsAttribute.GetMatchedTypes())
                    {
                        genericMenu.AddItem(new GUIContent(type.FullName.Replace('.','/')), false, () =>
                        {
                            assemblyQualifiedProperty.stringValue = type.AssemblyQualifiedName;
                            assemblyQualifiedProperty.serializedObject.ApplyModifiedProperties();
                        });
                    }
                    genericMenu.ShowAsContext();
                }
                else if (TypeReferenceConstraintsAttribute.IsNotNull())
                {
                    constraintsAttribute = TypeReferenceConstraintsAttribute;
                    var genericMenu = new GenericMenu();
                    genericMenu.AddItem(new GUIContent("Null"), false, () =>
                    {
                        assemblyQualifiedProperty.stringValue = null;
                        assemblyQualifiedProperty.serializedObject.ApplyModifiedProperties();    
                    });
                    
                    foreach (var type in constraintsAttribute.GetMatchedTypes())
                    {
                        genericMenu.AddItem(new GUIContent(type.FullName.Replace('.','/')), false, () =>
                        {
                            assemblyQualifiedProperty.stringValue = type.AssemblyQualifiedName;
                            assemblyQualifiedProperty.serializedObject.ApplyModifiedProperties();
                        });
                    }
                    genericMenu.ShowAsContext();
                    TypeReferenceConstraintsAttribute = null;
                }
                else if (property.FindPropertyRelative("_assignableType") is { } assignableTypeProperty && Type.GetType(assignableTypeProperty.stringValue) is {} type)
                {
                    var genericMenu = new GenericMenu();
                    genericMenu.AddItem(new GUIContent("Null"), false, () =>
                    {
                        assemblyQualifiedProperty.stringValue = null;
                        assemblyQualifiedProperty.serializedObject.ApplyModifiedProperties();    
                    });
                    
                    foreach (var VARIABLE in AppDomain.CurrentDomain
                        .GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes())
                        .Where(x => type.IsAssignableFrom(x)))
                    {
                        genericMenu.AddItem(new GUIContent(VARIABLE.FullName.Replace('.', '/')), false, () =>
                        {
                            assemblyQualifiedProperty.stringValue = VARIABLE.AssemblyQualifiedName;
                            assemblyQualifiedProperty.serializedObject.ApplyModifiedProperties();
                        });
                    }
                    
                    genericMenu.ShowAsContext();
                    TypeReferenceConstraintsAttribute = null;
                }
            }
        }
    }
}