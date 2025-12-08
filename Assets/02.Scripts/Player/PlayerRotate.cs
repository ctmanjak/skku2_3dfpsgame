using Core;
using UnityEngine;

namespace Player
{
    public class PlayerRotate : MonoBehaviour
    {
        [SerializeField] private CameraRotate _cameraRotate;
        [SerializeField] private float _sensitivity = 1f;

        private float _accumulateX;
        private float _accumulateY;

        private void Awake()
        {
            if (!_cameraRotate && Camera.main) _cameraRotate = Camera.main.GetComponent<CameraRotate>();
        }

        private void Update()
        {        
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");

            _accumulateX += mouseX * _sensitivity * Time.deltaTime;
            _accumulateY += mouseY * _sensitivity * Time.deltaTime;

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, _accumulateX);
            _cameraRotate.Rotate(_accumulateY, _accumulateX);
        }
    }
}
