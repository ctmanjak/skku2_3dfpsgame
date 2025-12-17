using Core;
using UnityEngine;

namespace Player
{
    public class PlayerRotate : MonoBehaviour
    {
        private const float MIN_Y = -90f;
        private const float MAX_Y = 90f;
        
        [SerializeField] private CameraRotate _cameraRotate;
        [SerializeField] private float _sensitivity = 1f;

        private float _accumulateX;
        private float _accumulateY;

        private void Awake()
        {
            if (!_cameraRotate && Camera.main) _cameraRotate = Camera.main.GetComponent<CameraRotate>();
        }

        public void LockAim(Vector2 axis)
        {
            
        }

        public void Aim(Vector2 axis)
        {
            float mouseX = axis.x;
            float mouseY = axis.y;

            _accumulateX += mouseX * _sensitivity * Time.deltaTime;
            _accumulateY = Mathf.Clamp(_accumulateY + mouseY * _sensitivity * Time.deltaTime, MIN_Y, MAX_Y);

            transform.rotation = Quaternion.Euler(0f, _accumulateX, 0f);
            _cameraRotate.SetBaseRotation(_accumulateY, _accumulateX);
        }
    }
}
