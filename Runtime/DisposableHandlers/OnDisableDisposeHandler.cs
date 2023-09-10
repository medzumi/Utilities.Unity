using System;
using System.Collections.Generic;
using medzumi.Utilities;
using UnityEngine;

namespace Utilities.Unity.DisposableHandlers
{
    [ExecuteAlways]
    public class OnDisableDisposeHandler : MonoBehaviour, IDisposeHandler
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public void OnDispose(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        private void OnDisable()
        {
            foreach (IDisposable disposable in _disposables)
            {
                disposable. Dispose();
            }
            _disposables.Clear();
        }
    }
}