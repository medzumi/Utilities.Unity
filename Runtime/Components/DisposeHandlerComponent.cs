using System;
using UnityEngine;

namespace Utilities.Unity.Components
{
    public class DisposeHandlerComponent : MonoBehaviour, IDisposeHandler, IDisposable
    {
        private readonly DisposeHandler _disposeHandler;

        public void Reset()
        {
            _disposeHandler.Reset();
        }

        public void Subscribe(IDisposable disposable)
        {
            _disposeHandler.Subscribe(disposable);
        }

        public void Dispose()
        {
            _disposeHandler.Dispose();
        }
    }
}