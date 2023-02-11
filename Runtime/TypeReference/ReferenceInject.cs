using System;
using UnityEngine;

namespace Utilities.Unity.TypeReference
{
    [Serializable]
    public struct ReferenceInject<T> : ISerializationCallbackReceiver
    {
        [SerializeField] [HideInInspector] private string _declareType;
        [SerializeReference]
        private object _value;

        public T GetValue()
        {
            return (T) _value;
        }

        public void OnBeforeSerialize()
        {
            _declareType = typeof(T).AssemblyQualifiedName;
        }

        public void OnAfterDeserialize()
        {
            _declareType = typeof(T).AssemblyQualifiedName;
        }
    }
}