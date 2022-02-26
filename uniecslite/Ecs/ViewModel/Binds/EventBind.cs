using System;
using System.Linq;
using ApplicationScripts.CodeExtensions;
using ApplicationScripts.ViewModel.Events;

namespace ApplicationScripts.ViewModel.Binds
{
    public abstract class EventBind : Bind
    {
    
    }
    
    public abstract class EventBind<T> : EventBind where T :struct
    {
        private static Type _defaultType;

        static EventBind()
        {
            _defaultType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t => t.IsSubclassDeep(typeof(EventBind<T>)) && !t.IsAbstract && !t.IsGenericType);
        }
        
        public static EventBind<T> GetDefault()
        {
            if (_defaultType.IsNotNull())
            {
                return Activator.CreateInstance(_defaultType) as EventBind<T>;
            }

            return null;
        }
        
        public override Type BindType => typeof(Action<T>);
        
        public abstract Event<T> Create();

        private Event<T> _cache;

        public Event<T> GetEvent
        {
            get => _cache ??= ViewModel?.GetEvent<T>(Key);
        }
        
        public override void ConnectTo(global::ApplicationScripts.ViewModel.ViewModel to)
        {
            var toEvent = GetOrCreate(Key, to);

            toEvent.OnEvent += GetEvent.PushData;
        }

        public override void ConnectFrom(global::ApplicationScripts.ViewModel.ViewModel @from)
        {
            var fromEvent = GetOrCreate(Key, from);
            GetEvent.OnEvent += fromEvent.PushData;
        }

        public override void DisconnectTo(global::ApplicationScripts.ViewModel.ViewModel to)
        {
            var toEvent = GetOrCreate(Key, to);
            toEvent.OnEvent -= GetEvent.PushData;
        }

        public override void DisconnectFrom(global::ApplicationScripts.ViewModel.ViewModel @from)
        {
            var fromEvent = GetOrCreate(Key, from);
            GetEvent.OnEvent -= fromEvent.PushData;
        }

        private Event<T> GetOrCreate(string key, global::ApplicationScripts.ViewModel.ViewModel viewModel)
        {
            var viewModelEvent = viewModel.GetEvent<T>(key);
            if (viewModelEvent.IsNull())
            {
                viewModelEvent = Create();
                viewModel.AddEvent(key, viewModelEvent);
            }

            return viewModelEvent;
        }
    }
}