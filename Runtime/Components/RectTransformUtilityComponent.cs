using System;
using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformUtilityComponent : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private Vector3 _cachedLocalScale;
        private Vector2 _cachedAnchoredPosition;
        private Vector2 _cachedSizeDelta;
        private Quaternion _cachedRotation;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
            Save();
        }

        private void OnBeforeTransformParentChanged()
        {
            Save();
        }

        private void OnTransformParentChanged()
        {
            Load();
        }

        private void Load()
        {
            _rectTransform.localRotation = _cachedRotation;
            _rectTransform.localScale = _cachedLocalScale;
            _rectTransform.anchoredPosition = _cachedAnchoredPosition;
            _rectTransform.sizeDelta = _cachedSizeDelta;
        }

        private void Save()
        {
            _cachedRotation = _rectTransform.localRotation;
            _cachedLocalScale = _rectTransform.localScale;
            _cachedAnchoredPosition = _rectTransform.anchoredPosition;
            _cachedSizeDelta = _rectTransform.sizeDelta;
        }
    }
}