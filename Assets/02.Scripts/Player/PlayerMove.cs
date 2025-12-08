using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _jumpPower = 5f;
        
        private Camera _camera;
        private CharacterController _controller;

        private float _velocityY;

        private void Awake()
        {
            _camera = Camera.main;
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            _velocityY += _gravity * Time.deltaTime;
            
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            
            Vector3 direction = new Vector3(x, 0, y);
            
            direction.Normalize();
            direction = transform.TransformDirection(direction);

            if (Input.GetButtonDown("Jump") && _controller.isGrounded)
            {
                _velocityY = _jumpPower;
            }
            
            direction.y = _velocityY;
            
            _controller.Move(direction * (_moveSpeed * Time.deltaTime));
        }
    }
}