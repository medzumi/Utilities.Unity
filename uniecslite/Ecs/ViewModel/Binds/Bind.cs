using System;
using UnityEngine;

namespace ApplicationScripts.ViewModel.Binds
{
    [Serializable]
    public abstract class Bind
    {
        public abstract Type BindType { get; }
        
        [SerializeField] private global::ApplicationScripts.ViewModel.ViewModel _viewModel;
        [SerializeField][BindString] private string _key;
        [SerializeField] private string _keyOfAnotherViewModel;

        public global::ApplicationScripts.ViewModel.ViewModel ViewModel => _viewModel;
        public string Key => _key;

        public abstract void ConnectTo(global::ApplicationScripts.ViewModel.ViewModel to);
        public abstract void ConnectFrom(global::ApplicationScripts.ViewModel.ViewModel from);

        public abstract void DisconnectTo(global::ApplicationScripts.ViewModel.ViewModel to);
        public abstract void DisconnectFrom(global::ApplicationScripts.ViewModel.ViewModel from);
    }
}