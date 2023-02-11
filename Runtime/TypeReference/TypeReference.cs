using System;
using UnityEngine;

namespace Utilities.Unity.TypeReference
{
    [Serializable]
    public class TypeReference : ISerializationCallbackReceiver
    {
        [SerializeField] private string _assemblyQualifiedName;

        public Type Type
        {
            get;
            private set;
        }
        public virtual void OnBeforeSerialize()
        {
            if (!string.IsNullOrWhiteSpace(_assemblyQualifiedName) && Type.GetType(_assemblyQualifiedName) is { } type)
            {
                Type = type;
            }
        }

        public virtual void OnAfterDeserialize()
        {
            if (!string.IsNullOrWhiteSpace(_assemblyQualifiedName) && Type.GetType(_assemblyQualifiedName) is { } type)
            {
                Type = type;
            }
        }
    }

    [Serializable]
    public class TypeReference<T> : TypeReference
    {
        [SerializeField] private string _assignableType;

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            _assignableType = typeof(T).AssemblyQualifiedName;
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            _assignableType = typeof(T).AssemblyQualifiedName;
        }
    }
}