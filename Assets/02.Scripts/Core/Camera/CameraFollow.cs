using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Core
{
    [Serializable]
    public struct ViewTypeConfig
    {
        public ECameraViewType ViewType;
        public Vector3 Offset;
    }
    
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private float _transitionSpeed = 5f;
        [SerializeField] private ViewTypeConfig[] _viewTypeConfigs;

        private Dictionary<ECameraViewType, Vector3> _viewTypeOffset = new();
        private List<ECameraViewType> _viewTypes = new();

        private int _currentViewTypeIndex;
        private float _transformTimer;
        
        private Vector3 _targetOffset;
        private Vector3 _currentPositionOffset;

        private Tweener _positionTween;

        private void Start()
        {
            foreach (var viewTypeConfig in _viewTypeConfigs)
            {
                _viewTypeOffset[viewTypeConfig.ViewType] = viewTypeConfig.Offset;
                _viewTypes.Add(viewTypeConfig.ViewType);
            }

            if (_currentPositionOffset == Vector3.zero)
            {
                _targetOffset = _viewTypeOffset[_viewTypes[_currentViewTypeIndex]];
                _currentPositionOffset = _viewTypeOffset[_viewTypes[_currentViewTypeIndex]];
            }
        }

        public void NextViewType()
        {
            if (++_currentViewTypeIndex >= _viewTypes.Count) _currentViewTypeIndex = 0;
            
            ECameraViewType currentViewType = _viewTypes[_currentViewTypeIndex];
            _targetOffset = _viewTypeOffset[currentViewType];

            float distance = (_currentPositionOffset - _targetOffset).magnitude;
            float duration = distance / _transitionSpeed;
            
            _positionTween?.Kill();
            _positionTween = DOTween.To(
                () => _currentPositionOffset,
                offset => _currentPositionOffset = offset,
                _targetOffset,
                duration
            ).SetUpdate(UpdateType.Manual)
            .SetEase(Ease.Linear)
            .OnComplete(() => _positionTween = null);
        }
        
        private void Update()
        {
            Vector3 targetPosition = _targetTransform.position;
            if (_positionTween != null) _positionTween.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);
            
            transform.position = targetPosition + _targetTransform.rotation * _currentPositionOffset;
        }
    }
}
