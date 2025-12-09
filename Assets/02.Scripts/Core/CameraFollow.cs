using DG.Tweening;
using UnityEngine;

namespace Core
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Transform _tpsTarget;
        [SerializeField] private float _transformDuration = 10f;

        private bool _isFirstPerson = true;

        private float _transformTimer;
        private Transform _targetTransform;
        private Transform _originTransform;

        private Tweener _transformTween;
        
        private void Update()
        {
            _targetTransform = _isFirstPerson ? _target : _tpsTarget;
            Vector3 targetPosition = _targetTransform.position;
            if (_transformTween != null)
            {
                _transformTimer += Time.deltaTime;
                // if (_transformTimer > _transformDuration) _transformTween.Done();
                _transformTween.ChangeValues(_originTransform.position, targetPosition, _transformDuration);
                _transformTween.ManualUpdate(_transformTimer, Time.unscaledDeltaTime);
            }
            else transform.position = targetPosition;
            
            if (Input.GetKeyDown(KeyCode.T))
            {
                _isFirstPerson = !_isFirstPerson;

                _transformTimer = 0;
                _originTransform = _targetTransform;
                
                _transformTween?.Done();
                
                _transformTween = DOTween.To(
                    () => transform.position,
                    pos => transform.position = pos,
                    targetPosition,
                    _transformDuration
                );
                _transformTween.SetUpdate(UpdateType.Manual);
                _transformTween.OnComplete(() => _transformTween = null);
            }
        }
    }
}
