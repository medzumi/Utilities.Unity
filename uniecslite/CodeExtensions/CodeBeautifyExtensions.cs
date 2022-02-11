using System;
using System.Collections.Generic;
using System.Reflection;

namespace ApplicationScripts.CodeExtensions
{
    public struct PropertyOrField
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly FieldInfo _fieldInfo;

        public PropertyOrField(PropertyInfo propertyInfo, FieldInfo fieldInfo)
        {
            _propertyInfo = propertyInfo;
            _fieldInfo = fieldInfo;
        }

        public T Get<T>(object value)
        {
            if (_propertyInfo != null)
                return (T)_propertyInfo.GetValue(value);
            if (_fieldInfo != null)
                return (T) _fieldInfo.GetValue(value);
            return default;
        }

        public string GetName()
        {
            if (_propertyInfo != null)
                return _propertyInfo.Name;
            if (_fieldInfo != null)
                return _fieldInfo.Name;
            return string.Empty;
        }

        public Type GetType()
        {
            if (_propertyInfo.IsNotNull())
                return _propertyInfo.PropertyType;
            if (_fieldInfo.IsNotNull())
                return _fieldInfo.FieldType;
            return null;
        }
    }
    
    public static class CodeBeautifyExtensions
    {
        public static bool IsNull<T>(this T obj)
        {
            return obj == null;
        }

        public static bool IsNotNull<T>(this T obj)
        {
            return obj != null;
        }

        public static bool IsNotDefault<T>(this T valueObj) where T : struct
        {
            return EqualityComparer<T>.Default.Equals(valueObj, default);
        }

        public static int IndexOf<T, TValue>(this T enumerable, TValue value) where T : IEnumerable<TValue>
        {
            var index = -1;
            foreach (var enumerableValue in enumerable)
            {
                index++;
                if (Comparer<TValue>.Default.Compare(enumerableValue, value) == 0)
                {
                    return index;
                }
            }

            return -1;
        }

        public static PropertyOrField GetPropertyOrField(this Type type, string name, BindingFlags additionalBindingFlags = BindingFlags.Default)
        {
            var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | additionalBindingFlags | BindingFlags.Instance);
            if (field.IsNotNull())
                return new PropertyOrField(null, field);
            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | additionalBindingFlags | BindingFlags.Instance);
            if (property != null)
                return new PropertyOrField(property, null);
            return default;
        }

        public static PropertyOrField GetPropertyOrField(this object obj, string name,
            BindingFlags additionalBindingFlags = BindingFlags.Default)
        {
            return obj.GetType().GetPropertyOrField(name, additionalBindingFlags);
        }

        public static List<PropertyOrField> GetPropertiesOrFields(this Type type, List<PropertyOrField> buffer = null)
        {
            if (buffer.IsNull())
                buffer = new List<PropertyOrField>();

            foreach (var VARIABLE in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                buffer.Add(new PropertyOrField(VARIABLE, null));
            }

            foreach (var VARIABLE in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                buffer.Add(new PropertyOrField(null, VARIABLE));
            }

            return buffer;
        }
    }
}