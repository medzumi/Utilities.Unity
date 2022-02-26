using System;
using System.Linq;
using ApplicationScripts.CodeExtensions;
using ApplicationScripts.Properties;

namespace ApplicationScripts.ViewModel.Binds
{
    public abstract class PropertyBind : Bind
    {
        public abstract ReactiveProperty GetPropertyBase();
    }
    
    public abstract class PropertyBind<T> : PropertyBind
    {
        private static Type _defaultType;

        static PropertyBind()
        {
            _defaultType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t => t.IsSubclassDeep(typeof(PropertyBind<T>)) && !t.IsAbstract && !t.IsGenericType);
        }
        
        public static PropertyBind<T> GetDefault()
        {
            if (_defaultType.IsNotNull())
            {
                return Activator.CreateInstance(_defaultType) as PropertyBind<T>;
            }

            return null;
        }
        
        public override Type BindType => typeof(T);

        private ReactiveProperty<T> _cache;

        protected abstract ReactiveProperty<T> Create();
        
        public ReactiveProperty<T> GetProperty()
        {
            return _cache ??= ViewModel?.GetProperty<T>(Key);
        }

        public override ReactiveProperty GetPropertyBase()
        {
            return _cache ??= ViewModel?.GetProperty<T>(Key);
        }

        public override void ConnectTo(global::ApplicationScripts.ViewModel.ViewModel to)
        {
            var toProperty = GetOrCreate(Key, to);
            toProperty.OnChange += GetProperty().SetValue;
        }

        public override void ConnectFrom(global::ApplicationScripts.ViewModel.ViewModel @from)
        {
            var fromProperty = GetOrCreate(Key, from);
            GetProperty().OnChange += fromProperty.SetValue;
        }

        public override void DisconnectTo(global::ApplicationScripts.ViewModel.ViewModel to)
        {
            var toProperty = GetOrCreate(Key, to);
            toProperty.OnChange -= GetProperty().SetValue;
        }

        public override void DisconnectFrom(global::ApplicationScripts.ViewModel.ViewModel @from)
        {
            var fromProperty = GetOrCreate(Key, from);
            GetProperty().OnChange -= fromProperty.SetValue;
        }

        private ReactiveProperty<T> GetOrCreate(string key, global::ApplicationScripts.ViewModel.ViewModel viewModel)
        {
            var property = viewModel.GetProperty<T>(key);
            if (property.IsNull())
            {
                property = Create();
                viewModel.AddProperty(key, property);
            }

            return property;
        }
    }
}