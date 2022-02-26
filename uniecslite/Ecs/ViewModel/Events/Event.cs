using System;
using System.Collections.Generic;

namespace ApplicationScripts.ViewModel.Events
{
    public class Event<T> where T : struct
    {
        private readonly object _locker = new object();

        public Action<T> PushData { get; }

        public Event()
        {
            PushData = PushDataHandler;
        }

        public event Action<T> OnEvent
        {
            add
            {
                lock (_locker)
                {
                    _subscribers.Add(value);
                }
            }
            remove
            {
                lock (_locker)
                {
                    _subscribers.Remove(value);
                }
            }
        }

        private readonly List<Action<T>> _subscribers = new List<Action<T>>();

        private void PushDataHandler(T data)
        {
            foreach (var action in _subscribers) action(data);
        }
    }
}