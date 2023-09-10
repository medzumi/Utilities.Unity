using System;
using UnityEngine;

namespace medzumi.utilities.unity
{
    [Serializable]
    public class UniNullable<T>
    {
        [SerializeField] private bool _hasValue;
        [SerializeField] private T _value;

        public UniNullable()
        {
            
        }

        public UniNullable(T value)
        {
            _value = value;
            _hasValue = true;
        }

        public bool HasValue => _hasValue;

        public T GetValue()
        {
            if (_hasValue)
            {
                return _value;
            }
            else
            {
                throw new InvalidOperationException("Doesn't contains value");
            }
        }

        public void SetValue(T? value)
        {
            if (value != null)
            {
                _hasValue = true;
                _value = value;
            }
            else
            {
                _hasValue = false;
            }
        }
    }
}