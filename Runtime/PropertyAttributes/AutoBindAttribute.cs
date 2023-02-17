using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.Unity.PropertyAttributes
{
    [Flags]
    public enum AutoBindType
    {
        Self = 0,
        Child = 1,
        Parent = 2
    }
    
    public class AutoBindAttribute : PropertyAttribute
    {
        public readonly AutoBindType Type;
        public readonly string Name;

        private readonly Stack<(int, Transform)> _stack = new Stack<(int, Transform)>();

        public AutoBindAttribute(AutoBindType autoBindType = AutoBindType.Self, string name = null)
        {
            Type = autoBindType;
            Name = name;
        }

        public Object AutoBind(Object obj, string type)
        {
            if (obj is Component component)
            {
                if ((Type & AutoBindType.Self) == AutoBindType.Self)
                {
                    var result = component.GetComponent(type);
                    if (result)
                    {
                        return result;
                    }
                }
                if ((Type & AutoBindType.Parent) == AutoBindType.Parent)
                {
                    if (string.IsNullOrWhiteSpace(Name))
                    {
                        var transform = component.transform.parent;
                        while (transform)
                        {
                            var result = transform.GetComponent(type);
                            if (result)
                                return result;
                            transform = transform.parent;
                        }
                    }
                }
                if ((Type & AutoBindType.Child) == AutoBindType.Child)
                {
                    if (string.IsNullOrWhiteSpace(Name))
                    {
                        lock (_stack)
                        {
                            var transform = component.transform;
                            for (int i = 0; i < transform.childCount; i++)
                            {
                                var child = transform.GetChild(i);
                                var result = child.GetComponent(type);
                                if (result)
                                {
                                    return result;
                                }
                                else if(_stack.Count == 0 || _stack.Peek().Item2 != child)
                                {
                                    _stack.Push((i, transform));
                                    transform = child;
                                    i = -1;
                                    continue;
                                }
                                else if(_stack.Count > 0)
                                {
                                    (i, transform) = _stack.Pop();
                                }
                            }

                            return null;
                        }
                    }
                    else
                    {
                        return component.transform
                                        .Find(Name)
                                        .GetComponent(type);
                    }
                }
            }

            return null;
        }
    }
}