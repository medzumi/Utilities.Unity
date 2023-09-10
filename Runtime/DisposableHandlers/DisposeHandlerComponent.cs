using System;
using medzumi.Utilities;
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

        public void OnDispose(IDisposable disposable)
        {
            _disposeHandler.OnDispose(disposable);
        }

        public void Dispose()
        {
            _disposeHandler.Dispose();
        }
    }
}