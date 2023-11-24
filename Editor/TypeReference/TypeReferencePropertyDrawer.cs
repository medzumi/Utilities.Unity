using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Utilities.Unity.TypeReference.Editor
{
    [CustomPropertyDrawer(typeof(TypeReferenceConstraintsAttribute))]
    [CustomPropertyDrawer(typeof(TypeReference), true)]
    public class TypeReferencePropertyDrawer : PropertyDrawer
    {
        private static readonly GUIStyle _prefixFullStyle = new GUIStyle(GUI.skin.label);
        private static readonly GUIStyle _prefixShortStyle = new GUIStyle(GUI.skin.label);
        private static readonly GUIStyle _prefixLabelStyle = new GUIStyle(GUI.skin.label);
        private static readonly GUIStyle _prefixDotStyle = new GUIStyle(EditorStyles.boldLabel);
        
        private static readonly GUIStyle _buttonStyle = new GUIStyle(GUI.skin.button);
        private static readonly GUIStyle _buttonLabelStyle = new GUIStyle(GUI.skin.label);
        private static readonly GUIStyle _buttonDotStyle = new GUIStyle(EditorStyles.boldLabel);

        private static readonly GUIContent _cachedGUIContent = new GUIContent();

        private static readonly Dictionary<string, TypeNames> _cachedTypeNames = new Dictionary<string, TypeNames>();
        
        public static TypeReferenceConstraintsAttribute TypeReferenceConstraintsAttribute;
        
        static TypeReferencePropertyDrawer()
        {
            _prefixFullStyle.alignment = TextAnchor.MiddleRight;
            _prefixFullStyle.wordWrap = false;
            _prefixShortStyle.alignment = TextAnchor.MiddleLeft;
            _prefixShortStyle.wordWrap = false;
            _prefixLabelStyle.alignment = TextAnchor.MiddleRight;
            _prefixLabelStyle.wordWrap = false;
            _prefixLabelStyle.normal.textColor = new Color(0.65f, 0.65f, 0.65f);
            _prefixDotStyle.alignment = TextAnchor.MiddleRight;
            _prefixDotStyle.wordWrap = false;
            _prefixDotStyle.normal.background = Texture2D.grayTexture;
            
            _buttonStyle.alignment = TextAnchor.MiddleRight;
            _buttonLabelStyle.alignment = TextAnchor.MiddleRight;
            _buttonLabelStyle.wordWrap = false;
            _buttonLabelStyle.normal.textColor = new Color(0.75f, 0.75f, 0.75f);
            _buttonDotStyle.alignment = TextAnchor.MiddleRight;
            _buttonDotStyle.wordWrap = false;
            var texture = new Texture2D(1, 1); 
            texture.SetPixel(0, 0, new Color(0.45f, 0.45f, 0.45f));
            texture.Apply();
            _buttonDotStyle.normal.background = texture;
            _buttonDotStyle.normal.textColor = Color.white;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawPrefixLabel(ref position, label);

            var assemblyQualifiedProperty = property.FindPropertyRelative("_assemblyQualifiedName");
            var propertyType = Type.GetType(assemblyQualifiedProperty.stringValue);

            _cachedGUIContent.text = propertyType?.Name;
            _cachedGUIContent.tooltip = propertyType?.Assembly.FullName;
            
            if (GUI.Button(position, _cachedGUIContent, _buttonStyle))
            {
                DrawButtonExpanded(property, assemblyQualifiedProperty);
            }
            
            DrawButtonLabel(position, propertyType, _buttonStyle.CalcSize(_cachedGUIContent));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, label);     
        }
        
        private static void DrawPrefixLabel(ref Rect position, GUIContent label)
        {
            if (!_cachedTypeNames.TryGetValue(label.text, out var cachedType))
            {
                cachedType = new TypeNames(label.text);
                _cachedTypeNames.Add(label.text, cachedType);
            }

            _cachedGUIContent.text = cachedType.FullName;
            var prefixSize = _prefixFullStyle.CalcSize(_cachedGUIContent);
            var prefixPosition = position;
            var prefixWidth = EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 15f;
            prefixPosition.width = Mathf.Min(prefixSize.x, prefixWidth);
            
            _cachedGUIContent.tooltip = cachedType.NameTooltip;
            if (prefixSize.x > prefixWidth)
            {
                _cachedGUIContent.text = ".";
                var dotSize = _prefixDotStyle.CalcSize(_cachedGUIContent);
                var dotPosition = prefixPosition;
                dotPosition.width = dotSize.x;
                GUI.Label(dotPosition, _cachedGUIContent, _prefixDotStyle);
                
                prefixPosition.x += dotSize.x;
                _cachedGUIContent.text = cachedType.Name;
                prefixSize = _prefixShortStyle.CalcSize(_cachedGUIContent);
                prefixPosition.width = prefixSize.x;
                GUI.Label(prefixPosition, _cachedGUIContent, _prefixShortStyle);
                prefixSize.x += dotSize.x;
            }
            else
            {
                GUI.Label(prefixPosition, _cachedGUIContent, _prefixFullStyle);
            }

            DrawPrefixInnerLabel(prefixPosition, prefixWidth - prefixSize.x, cachedType);

            position.x += prefixWidth;
            position.width -= prefixWidth;
        }

        private static void DrawPrefixInnerLabel(Rect position, float width, TypeNames cachedType)
        {
            _cachedGUIContent.text = cachedType.AssemblyName;
            _cachedGUIContent.tooltip = cachedType.AssemblyTooltip;
            position.x += position.width;
            position.width = width;
            GUI.Label(position, _cachedGUIContent, _prefixLabelStyle);
        }

        private static void DrawButtonLabel(Rect position, Type propertyType, Vector2 buttonSize)
        {
            _cachedGUIContent.text = propertyType?.Namespace;
            var labelSize = _buttonLabelStyle.CalcSize(_cachedGUIContent);
            position.x += 2f;
            var buttonWidth = position.width - buttonSize.x;
            position.width = Mathf.Min(buttonWidth, labelSize.x);
            GUI.Label(position, _cachedGUIContent, _buttonLabelStyle);

            if (buttonWidth > labelSize.x)
            {
                return;
            }

            _cachedGUIContent.text = ".";
            position.x += position.width - 1.25f;
            position.width = _buttonDotStyle.CalcSize(_cachedGUIContent).x;
            GUI.Label(position, _cachedGUIContent, _buttonDotStyle);
        }
        
        private void DrawButtonExpanded(SerializedProperty property, SerializedProperty assemblyQualifiedProperty)
        {
            var types = GetMatchedTypes(property, out var clearStaticAttribute);
            if(types == null)
            {
                return;
            }
            
            var genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Null"), false, () =>
            {
                assemblyQualifiedProperty.stringValue = null;
                assemblyQualifiedProperty.serializedObject.ApplyModifiedProperties();
            });

            foreach (var type in types)
            {
                genericMenu.AddItem(new GUIContent(type.FullName.Replace('.', '/')), false, () =>
                {
                    assemblyQualifiedProperty.stringValue = type.AssemblyQualifiedName;
                    assemblyQualifiedProperty.serializedObject.ApplyModifiedProperties();
                });
            }

            genericMenu.ShowAsContext();

            if (clearStaticAttribute)
            {
                TypeReferenceConstraintsAttribute = null;
            }
        }

        private IEnumerable<Type> GetMatchedTypes(SerializedProperty property, out bool clearStaticAttribute)
        {
            clearStaticAttribute = false;
            if (this.attribute is TypeReferenceConstraintsAttribute attribute)
            {
                return attribute.GetMatchedTypes();
            }

            if (TypeReferenceConstraintsAttribute != null)
            {
                clearStaticAttribute = true;
                return TypeReferenceConstraintsAttribute.GetMatchedTypes();
            }

            var assignableTypeProperty = property.FindPropertyRelative("_assignableType");
            if (assignableTypeProperty == null)
            {
                return null;
            }

            var assignableType = Type.GetType(assignableTypeProperty.stringValue);
            if (assignableType == null)
            {
                return null;
            }

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(x => assignableType.IsAssignableFrom(x));
        }

        private struct TypeNames
        {
            private static readonly Regex _typeRegex = new Regex(@"(.+?)(,\s|$)");
            private static readonly StringBuilder _sb = new StringBuilder();
            
            public readonly string Name;
            public readonly string FullName;
            public readonly string AssemblyName;
            
            public readonly string NameTooltip;
            public readonly string AssemblyTooltip;

            public TypeNames(string assemblyQualifiedName)
            {
                var matches = _typeRegex.Matches(assemblyQualifiedName);
                FullName = matches[0].Groups[1].Value;
                var namespaceIndex = FullName.LastIndexOf('.');
                Name = FullName.Substring(namespaceIndex == -1 ? 0 : namespaceIndex + 1);
                AssemblyName = matches[1].Groups[1].Value;

                NameTooltip = FullName;
                
                _sb.Clear();
                _sb.Append(matches[1].Value);
                for (var index = 2; index < matches.Count; index++)
                {
                    _sb.AppendLine().Append(matches[index].Groups[1].Value);
                }
                AssemblyTooltip = _sb.ToString();
            }
        }
    }
}