using System;
using UnityEngine;
using Utilities.Unity.Services;

namespace Utilities.Unity.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class PivotFixer : MonoBehaviour
    {
        private IDisposable _disposable;
        private Action _updateAction;

        private RectTransform _rectTransform;
        private RectTransform _parentRectTransform;
        private Vector2 _defaultPivot;

        private void Awake()
        {
            _updateAction = UpdateAction;
            _rectTransform = transform as RectTransform;
            _defaultPivot = _rectTransform.pivot;
            _parentRectTransform = _rectTransform.parent as RectTransform;
        }

        private void OnTransformParentChanged()
        {
            _parentRectTransform = _rectTransform.parent as RectTransform;
        }

        private void UpdateAction()
        {
            if (_parentRectTransform)
            {
                int count = 0;
                var parentPosition = Devide(_parentRectTransform.position, _parentRectTransform.lossyScale);
                var position = Devide(_rectTransform.position, _rectTransform.lossyScale);
                if (parentPosition.x + _parentRectTransform.rect.xMin <
                    position.x + _rectTransform.rect.xMin)
                {
                    _rectTransform.pivot = new Vector2(0, _rectTransform.pivot.y);
                    count++;
                }

                if (parentPosition.x + _parentRectTransform.rect.xMax >
                    position.x + _rectTransform.rect.xMax && count != 1)
                {
                    _rectTransform.pivot = new Vector2(1, _rectTransform.pivot.y);
                }
                
                count = 0;
                
                if (parentPosition.y + _parentRectTransform.rect.yMin <
                    position.y + _rectTransform.rect.yMin)
                {
                    _rectTransform.pivot = new Vector2(_rectTransform.pivot.x , 1);
                    count++;
                }

                if (parentPosition.y + _parentRectTransform.rect.yMax >
                    position.y + _rectTransform.rect.yMax && count != 1)
                {
                    _rectTransform.pivot = new Vector2(_rectTransform.pivot.x, 0);
                }
            }
        }

        private Vector2 Devide(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }

        private void OnEnable()
        {
            _disposable = UpdateService.SubscribeToUpdate(_updateAction);
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }
    }
}