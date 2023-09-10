using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utilities.Unity.TypeReference
{
    public class TypeReferenceConstraintsAttribute : TypeConstraints
    {
        public Type BaseType;
        public Type[] InterfaceTypes;
        public Type[] AttributeTypes;
        private Type[] _matchedTypes;
        private bool _isPrewarmed = false;

        public TypeReferenceConstraintsAttribute(Type baseType = null, params Type[] types)
        {
            BaseType = baseType ?? typeof(object);
            InterfaceTypes = types
                .Where(type => type.IsInterface)
                .ToArray();
            AttributeTypes = types
                .Where(type => typeof(Attribute).IsAssignableFrom(type))
                .ToArray();
            _isPrewarmed = true;
        }

        public TypeReferenceConstraintsAttribute(params Type[] types)
        {
            BaseType = types.FirstOrDefault(t => !t.IsInterface) ?? typeof(object);
            InterfaceTypes = types
                .Where(type => type.IsInterface)
                .ToArray();
            AttributeTypes = types
                .Where(type => typeof(Attribute).IsAssignableFrom(type))
                .ToArray();
            _isPrewarmed = true;
        }

        public bool IsMatched(Type type)
        {
            return BaseType.IsAssignableFrom(type)
                   && InterfaceTypes.All(interfaceType => interfaceType.IsAssignableFrom(type))
                   && AttributeTypes.All(attributeType => type.GetCustomAttribute(attributeType) != null)
                   && !type.IsGenericTypeDefinition;
        }

        public override IEnumerable<Type> GetMatchedTypes(Type fieldInfo)
        {
            return _matchedTypes ??= AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => IsMatched(type) && type.IsAssignableFrom(type))
                .ToArray();
        }
    }
}