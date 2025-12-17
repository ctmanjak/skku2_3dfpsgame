using DG.Tweening;
using UnityEngine;

namespace Core
{
    public class CameraRotate : MonoBehaviour
    {
        [SerializeField] private float _restoreDuration;
        [SerializeField] private float _transitionSpeed = 5f;
        
        private float _baseX;
        private float _baseY;
        
        private float _recoilX;
        private float _recoilY;
        
        private float _restoreTimer;

        private bool _isRotationLock;
        private Tweener _rotationTween;

        public float BaseX => _baseX;
        public float BaseY => _baseY;

        public void AddRecoil(float xPower, float yPower)
        {
            _recoilX += yPower;
            _recoilY += xPower;

            _restoreTimer = _restoreDuration;
        }

        public void SetBaseRotation(float x, float y)
        {
            _baseX = x;
            _baseY = y;
        }

        public void LockRotation(Vector3 rotation, float duration)
        {
            _isRotationLock = true;
            
            _rotationTween?.Kill();
            _rotationTween = transform
                .DORotate(rotation, duration)
                .SetEase(Ease.Linear)
                .SetUpdate(UpdateType.Manual)
                .OnComplete(() => _rotationTween = null);
        }

        public void UnlockRotation()
        {
            _rotationTween?.Kill();
            _rotationTween = null;
            _isRotationLock = false;
        }

        public void Update()
        {
            if (_restoreTimer > 0f) _restoreTimer -= Time.deltaTime;
            _rotationTween?.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);

            if (_isRotationLock) return;
            
            _recoilX = Mathf.Lerp(_recoilX, 0f, (_restoreDuration - _restoreTimer) / _restoreDuration);
            _recoilY = Mathf.Lerp(_recoilY, 0f, (_restoreDuration - _restoreTimer) / _restoreDuration);

            float finalX = _baseX + _recoilX;
            float finalY = _baseY + _recoilY;
            transform.rotation = Quaternion.Euler(finalX, finalY, 0f);
        }
    }
}
