using System;
using UnityEditor;

namespace Utilities.Unity.TypeReference.Editor
{
    public interface ITypeSetter
    {
        void SetType(Type type, SerializedProperty serializedProperty);
    }
}