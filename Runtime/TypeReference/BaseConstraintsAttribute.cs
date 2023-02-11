using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Utilities.Unity.TypeReference
{
    public class BaseConstraintsAttribute : PropertyAttribute
    {
        public Type BaseType;
        public Type[] InterfaceTypes;
        public Type[] AttributeTypes;
        private Type[] _matchedTypes;

        public BaseConstraintsAttribute(Type baseType = null, params Type[] types)
        {
            BaseType = baseType ?? typeof(object);
            InterfaceTypes = types
                .Where(type => type.IsInterface)
                .ToArray();
            AttributeTypes = types
                .Where(type => typeof(Attribute).IsAssignableFrom(type))
                .ToArray();
        }

        public BaseConstraintsAttribute(params Type[] types)
        {
            BaseType = types.FirstOrDefault(t => !t.IsInterface) ?? typeof(object);
            InterfaceTypes = types
                .Where(type => type.IsInterface)
                .ToArray();
            AttributeTypes = types
                .Where(type => typeof(Attribute).IsAssignableFrom(type))
                .ToArray();
        }

        public bool IsMatched(Type type)
        {
            return BaseType.IsAssignableFrom(type)
                   && InterfaceTypes.All(interfaceType => interfaceType.IsAssignableFrom(type))
                   && AttributeTypes.All(attributeType => type.GetCustomAttribute(attributeType) != null);
        }

        public IReadOnlyCollection<Type> GetMatchedTypes()
        {
            return _matchedTypes ?? (_matchedTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => IsMatched(type))
                .ToArray());
        }
    }

    public class TypeReferenceConstraintsAttribute : BaseConstraintsAttribute
    {
        public TypeReferenceConstraintsAttribute(Type baseType = null, params Type[] types) : base(baseType, types)
        {
            
        }

        public TypeReferenceConstraintsAttribute(params Type[] types) : base(types)
        {
            
        }
    }

    public class SerializeReferenceConstraintsAttribute : BaseConstraintsAttribute
    {
        public SerializeReferenceConstraintsAttribute(Type baseType = null, params Type[] types) : base(baseType, types)
        {
            
        }
    }
}