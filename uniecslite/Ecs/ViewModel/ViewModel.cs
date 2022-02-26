using System;
using System.Collections.Generic;
using ApplicationScripts.Properties;
using ApplicationScripts.ViewModel.Events;
using JetBrains.Annotations;
using UnityEngine;

namespace ApplicationScripts.ViewModel
{
    public sealed class ViewModel : MonoBehaviour
    {
        [Serializable]
        public class Pair
        {
            public string key = String.Empty;
            [SerializeReference] public object property = new object();
        }
    
        [SerializeReference] private List<Pair> _propertiesList = new List<Pair>();

        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        private string _keyBuffer;

        private Predicate<Pair> _predicate;

        private void Awake()
        {
            _predicate = (p) => p.key.Equals(_keyBuffer);
        }

        [CanBeNull]
        public ReactiveProperty<T> GetProperty<T>(string key)
        {
            _keyBuffer = key;
            return _propertiesList.Find(_predicate).property as ReactiveProperty<T>;
        }

        [CanBeNull]
        public Event<T> GetEvent<T>(string key) where T :struct
        {
            _keyBuffer = key;
            return _propertiesList.Find(_predicate).property as Event<T>;
        }

        public void AddProperty<T>(string key, ReactiveProperty<T> property)
        {
            _propertiesList.Add(new Pair(){ key = key, property = property});
        }

        public void AddEvent<T>(string key, Event<T> @event) where T : struct
        {
            _propertiesList.Add(new Pair(){key =  key, property = @event});
        }
    }
}