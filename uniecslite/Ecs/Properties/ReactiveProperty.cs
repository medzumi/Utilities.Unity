using System;
using UnityEngine;

namespace ApplicationScripts.Properties
{
    public abstract class ReactiveProperty
    {
        public abstract event Action<string> OnChangeAsString;
    }

    [Serializable]
    public class ReactiveProperty<T> : ReactiveProperty
    {
        private event Action<T> _onChange = delegate(T obj) {  };

        public event Action<T> OnChange
        {
            add
            {
                _onChange += value;
                value(Value);
            }
            remove => _onChange -= value;
        }
    
        [SerializeField] private T value;

        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                _onChange(value);
                _onChangeAsString?.Invoke(value.ToString());
            }
        }

        public void SetValue(T value)
        {
            Value = value;
        }

        private event Action<string> _onChangeAsString; 

        public override event Action<string> OnChangeAsString
        {
            add
            {
                _onChangeAsString += value;
                value?.Invoke(Value.ToString());
            }
            remove => _onChangeAsString -= value;
        }
    }
}