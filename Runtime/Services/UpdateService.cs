using System;
using System.Collections.Generic;
using medzumi.Utilities.Pooling;
using UnityEngine;

namespace Utilities.Unity.Services
{
    public class UpdateService
    {
        private static UpdateServiceComponent _updateServiceComponent;

        private static UpdateServiceComponent GetUpdateServiceComponent()
        {
            if (!_updateServiceComponent)
            {
                _updateServiceComponent = (new GameObject("Updater")).AddComponent<UpdateServiceComponent>();
                GameObject.DontDestroyOnLoad(_updateServiceComponent.gameObject);
            }

            return _updateServiceComponent;
        } 
        
        public static IDisposable SubscribeToUpdate(Action action)
        {
            var updater = GetUpdateServiceComponent();
            var disposer = Disposer.Create();
            disposer.List = updater.UpdateActions;
            disposer.Node = updater.UpdateActions.AddLast(action);

            return disposer;
        }
        
        public static IDisposable SubscribeToLateUpdate(Action action)
        {
            var updater = GetUpdateServiceComponent();
            var disposer = Disposer.Create();
            disposer.List = updater.LateUpdateAction;
            disposer.Node = updater.LateUpdateAction.AddLast(action);

            return disposer;
        }
        
        public static IDisposable SubscribeToFixUpdate(Action action)
        {
            var updater = GetUpdateServiceComponent();
            var disposer = Disposer.Create();
            disposer.List = updater.FixUpdateAction;
            disposer.Node = updater.FixUpdateAction.AddLast(action);

            return disposer;
        }
        
        private class Disposer : PoolableObject<Disposer>
        {
            public LinkedList<Action> List;
            public LinkedListNode<Action> Node;

            protected override void DisposeHandler()
            {
                base.DisposeHandler();
                List.Remove(Node);
                List = null;
                Node = null;
            }
        }
    
        [ExecuteAlways]
        private class UpdateServiceComponent : MonoBehaviour
        {
            public readonly LinkedList<Action> UpdateActions = new LinkedList<Action>();
            public readonly LinkedList<Action> LateUpdateAction = new LinkedList<Action>();
            public readonly LinkedList<Action> FixUpdateAction = new LinkedList<Action>();

            private void Update()
            {
                foreach (var action in UpdateActions)
                {
                    action?.Invoke();
                }
            }

            private void LateUpdate()
            {
                foreach (var action in LateUpdateAction)
                {
                    action?.Invoke();
                }
            }

            private void FixedUpdate()
            {
                foreach (var action in FixUpdateAction)
                {
                    action?.Invoke();
                }
            }
        }
    }
}