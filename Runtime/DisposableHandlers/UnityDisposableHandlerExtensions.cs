using System;
using UnityEngine;
using Utilities.Unity.Extensions;

namespace Utilities.Unity.DisposableHandlers
{
    public static class UnityDisposableHandlerExtensions
    {
        public static IDisposable AddToDisable(this IDisposable disposable, GameObject gameObject)
        {
            gameObject.GetOrAddComponent<OnDisableDisposeHandler>().OnDispose(disposable);
            return disposable;
        }

        public static IDisposable AddToDestroy(this IDisposable disposable, GameObject gameObject)
        {
            gameObject.GetOrAddComponent<OnDestroyDisposeHandler>().OnDispose(disposable);
            return disposable;
        }
    }
}