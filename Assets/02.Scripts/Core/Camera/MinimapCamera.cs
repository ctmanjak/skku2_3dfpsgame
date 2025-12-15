using UnityEngine;

namespace Core
{
    public class MinimapCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _offsetY = 10f;
        [SerializeField] private float _minOrthographicSize = 10f;
        [SerializeField] private float _maxOrthographicSize = 50f;

        [SerializeField] private Camera _minimapCamera;
        
        private void LateUpdate()
        {
            Vector3 targetPosition = _target.position;
            Vector3 finalPosition = targetPosition + new Vector3(0f, _offsetY, 0f);
            transform.position = finalPosition;

            Vector3 targetAngle = _target.eulerAngles;
            targetAngle.x = 90f;

            transform.eulerAngles = targetAngle;
        }
        
        public void ZoomOut(float amount)
        {
            SetOrthographicSize(_minimapCamera.orthographicSize + amount);
        }
        
        public void ZoomIn(float amount)
        {
            SetOrthographicSize(_minimapCamera.orthographicSize - amount);
        }

        private void SetOrthographicSize(float orthographicSize)
        {
            _minimapCamera.orthographicSize = Mathf.Clamp(orthographicSize, _minOrthographicSize, _maxOrthographicSize);
        }
    }
}