using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using medzumi.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Utilities.Unity.Editor;

namespace Utilities.Unity.TypeReference.Editor
{
    public class TypeReferenceSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private readonly List<SearchTreeEntry> _searchTreeEntries = new List<SearchTreeEntry>();
        private SerializedProperty _serializedProperty;
        private ITypeConstraints _typeReferenceConstraintsAttribute;
        private HashSet<string> _hashSet = new HashSet<string>();
        private StringBuilder _stringBuilder = new StringBuilder();
        private string _maxLengthName = String.Empty;
        private ITypeSetter _typeSetter;

        public float MaxWidth => UnityEditor.EditorStyles.label.CalcSize(new GUIContent(_maxLengthName)).x;

        private void OnEnable()
        {
            _stringBuilder.Clear();
            _searchTreeEntries.Clear();
            _hashSet.Clear();
            _maxLengthName = String.Empty;
        }

        public void Initialize(SerializedProperty serializedProperty,
            ITypeConstraints typeReferenceConstraintsAttribute, ITypeSetter typeSetter, Type assignableType)
        {
            if (!ReferenceEquals(_typeReferenceConstraintsAttribute, typeReferenceConstraintsAttribute))
            {
                _searchTreeEntries.Clear();
                _hashSet.Clear();
                _typeReferenceConstraintsAttribute = typeReferenceConstraintsAttribute;
                _maxLengthName = string.Empty;
                _searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent("Find type"), 0));
                _searchTreeEntries.Add(new SearchTreeEntry(new GUIContent("None"))
                {
                    level = 1,
                    userData = this
                });
                if (_typeReferenceConstraintsAttribute != null)
                {
                    foreach (var type in _typeReferenceConstraintsAttribute.GetMatchedTypes(assignableType))
                    {
                        if (type != null && !string.IsNullOrWhiteSpace(type.FullName))
                        {
                            if (_maxLengthName.Length < type.FullName.Length)
                            {
                                _maxLengthName = type.FullName;
                            }

                            var pathElements = new string[]{type.GetBeautifulFullName(false)};
                            if (pathElements.Length == 1)
                            {
                                _searchTreeEntries.Add(new SearchTreeEntry(new GUIContent(pathElements[0]))
                                {
                                    level = 1,
                                    userData = type
                                });
                            }
                            else
                            {
                                _stringBuilder.Clear();
                                for (int i = 0; i < pathElements.Length; i++)
                                {
                                    if (i != 0)
                                    {
                                        _stringBuilder.Append('.');
                                    }

                                    _stringBuilder.Append(pathElements[i]);
                                    var result = _stringBuilder.ToString();
                                    if (i != pathElements.Length - 1)
                                    {
                                        if (!_hashSet.Contains(result))
                                        {
                                            _searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent(result),
                                                i + 1));
                                            _hashSet.Add(result);
                                        }
                                    }
                                    else
                                    {
                                        _searchTreeEntries.Add(new SearchTreeEntry(new GUIContent(type.Name))
                                        {
                                            level = i + 1,
                                            userData = type
                                        });
                                    }
                                }
                            }

                            if (_searchTreeEntries.Count == Int32.MaxValue)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()))
                    {
                        if (type != null && !string.IsNullOrWhiteSpace(type.FullName))
                        {
                            if (_maxLengthName.Length < type.FullName.Length)
                            {
                                _maxLengthName = type.FullName;
                            }

                            var pathElements = type.FullName.Split('.');
                            if (pathElements.Length == 1)
                            {
                                _searchTreeEntries.Add(new SearchTreeEntry(new GUIContent(type.FullName))
                                {
                                    level = 1,
                                    userData = type
                                });
                            }
                            else
                            {
                                _stringBuilder.Clear();
                                for (int i = 0; i < pathElements.Length; i++)
                                {
                                    if (i != 0)
                                    {
                                        _stringBuilder.Append('.');
                                    }

                                    _stringBuilder.Append(pathElements[i]);
                                    var result = _stringBuilder.ToString();
                                    if (i != pathElements.Length - 1)
                                    {
                                        if (!_hashSet.Contains(result))
                                        {
                                            _searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent(result),
                                                i + 1));
                                            _hashSet.Add(result);
                                        }
                                    }
                                    else
                                    {
                                        _searchTreeEntries.Add(new SearchTreeEntry(new GUIContent(result))
                                        {
                                            level = i + 1,
                                            userData = type
                                        });
                                    }
                                }
                            }

                            if (_searchTreeEntries.Count == Int32.MaxValue)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            _serializedProperty = serializedProperty;
            _typeSetter = typeSetter;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            return _searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            if (SearchTreeEntry.userData is Type { } type && _serializedProperty != null)
            {
                _typeSetter.SetType(type, _serializedProperty);
                return true;
            }
            else if (SearchTreeEntry.userData == this)
            {
                _typeSetter.SetType(null, _serializedProperty);
                return true;
            }

            return false;
        }
    }
}