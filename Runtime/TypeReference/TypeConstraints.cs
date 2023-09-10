using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Utilities.Unity.TypeReference
{
    public abstract class TypeConstraints : PropertyAttribute, ITypeConstraints
    {
        public abstract IEnumerable<Type> GetMatchedTypes(Type assignableType);
    }

    public interface ITypeConstraints
    {
        IEnumerable<Type> GetMatchedTypes(Type assignableType);
    }
}