using System;
using ApplicationScripts.CodeExtensions;
using ApplicationScripts.Properties;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ApplicationScripts.ViewModel.ReflectionBind
{
    public class ReflectionBindStringAttribute : PropertyAttribute
    {
    }
    
    [Serializable]
    public abstract class ReflectionBind
    {
        [SerializeField] protected Object _object;
        [SerializeField][ReflectionBindString] protected string _propertyName;

        public abstract ReactiveProperty GetReactivePropertyBase();
    }
    
    [Serializable]
    public class ReflectionBind<T> : ReflectionBind  where T : struct
    {
        [SerializeField][HideInInspector] private string _type = typeof(T).AssemblyQualifiedName;
        private ReactiveProperty<T> _cache;
        
        private PropertyOrField _propertyInfo;

        public ReactiveProperty<T> GetProperty()
        {
            return _cache ??= GetPropertyHandler();
        }

        private ReactiveProperty<T> GetPropertyHandler()
        {
            var propertyOrField = _object.GetPropertyOrField(_propertyName);
            return propertyOrField.Get<ReactiveProperty<T>>(_object);
        }

        public override ReactiveProperty GetReactivePropertyBase()
        {
            return GetProperty();
        }
    }
}