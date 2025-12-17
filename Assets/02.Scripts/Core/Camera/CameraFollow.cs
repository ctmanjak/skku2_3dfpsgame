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
        public bool IsRotationLock;
        public Vector3 Rotation;
    }
    
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private float _transitionSpeed = 5f;
        [SerializeField] private ViewTypeConfig[] _viewTypeConfigs;

        private Dictionary<ECameraViewType, ViewTypeConfig> _viewTypeConfig = new();
        private List<ECameraViewType> _viewTypes = new();

        private int _currentViewTypeIndex;
        private float _transformTimer;
        private float _transformDuration;
        
        private Vector3 _targetOffset;
        private Vector3 _currentPositionOffset;

        private Tweener _positionTween;

        public float TransformDuration => _transformDuration;

        private void Start()
        {
            foreach (var viewTypeConfig in _viewTypeConfigs)
            {
                _viewTypeConfig[viewTypeConfig.ViewType] = viewTypeConfig;
                _viewTypes.Add(viewTypeConfig.ViewType);
            }

            if (_currentPositionOffset == Vector3.zero)
            {
                _targetOffset = _viewTypeConfig[_viewTypes[_currentViewTypeIndex]].Offset;
                _currentPositionOffset = _viewTypeConfig[_viewTypes[_currentViewTypeIndex]].Offset;
            }
        }

        public ViewTypeConfig GetViewTypeConfig()
        {
            return _viewTypeConfig[_viewTypes[_currentViewTypeIndex]];
        }

        public void NextViewType()
        {
            if (++_currentViewTypeIndex >= _viewTypes.Count) _currentViewTypeIndex = 0;
            
            ECameraViewType currentViewType = _viewTypes[_currentViewTypeIndex];
            _targetOffset = _viewTypeConfig[currentViewType].Offset;

            float distance = (_currentPositionOffset - _targetOffset).magnitude;
            _transformDuration = distance / _transitionSpeed;
            
            _positionTween?.Kill();
            _positionTween = DOTween.To(
                () => _currentPositionOffset,
                offset => _currentPositionOffset = offset,
                _targetOffset,
                _transformDuration
            ).SetUpdate(UpdateType.Manual)
            .SetEase(Ease.Linear)
            .OnComplete(() => _positionTween = null);
        }
        
        private void Update()
        {
            Vector3 targetPosition = _targetTransform.position;
            _positionTween?.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);

            transform.position = targetPosition + _targetTransform.rotation * _currentPositionOffset;
        }
    }
}
