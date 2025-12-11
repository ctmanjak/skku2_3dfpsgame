using UnityEngine;

namespace Core
{
    public class CameraRotate : MonoBehaviour
    {
        [SerializeField] private float _restoreDuration;
        
        private float _baseX;
        private float _baseY;
        
        private float _recoilX;
        private float _recoilY;
        
        private float _restoreTimer;
        
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

        public void Update()
        {
            if (_restoreTimer > 0f) _restoreTimer -= Time.deltaTime;
            
            _recoilX = Mathf.Lerp(_recoilX, 0f, (_restoreDuration - _restoreTimer) / _restoreDuration);
            _recoilY = Mathf.Lerp(_recoilY, 0f, (_restoreDuration - _restoreTimer) / _restoreDuration);

            float finalX = _baseX + _recoilX;
            float finalY = _baseY + _recoilY;
            transform.rotation = Quaternion.Euler(finalX, finalY, 0f);
        }
    }
}
