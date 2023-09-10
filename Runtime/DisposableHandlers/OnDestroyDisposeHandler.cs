using System;
using System.Collections.Generic;
using medzumi.Utilities;
using UnityEngine;

namespace Utilities.Unity.DisposableHandlers
{
    [ExecuteAlways]
    public class OnDestroyDisposeHandler : MonoBehaviour, IDisposeHandler
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private void OnDestroy()
        {
            foreach (IDisposable disposable in _disposables)
            {
                disposable.Dispose();
            }
            _disposables.Clear();
        }

        public void OnDispose(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }
    }
}