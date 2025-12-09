using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMove : MonoBehaviour
    {
        private const float GRAVITY = -9.81f;
        
        private float _moveSpeed;
        private float _jumpPower = 5f;
        private float _sprintMultiplier = 2f; 
        
        private CharacterController _controller;

        private float _velocityY;

        private bool _isSprinting;

        public bool IsSprinting => _isSprinting;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        public void Initialize(float moveSpeed, float jumpPower, float sprintMultiplier)
        {
            _moveSpeed = moveSpeed;
            _jumpPower = jumpPower;
            _sprintMultiplier = sprintMultiplier;
        }

        public void Move(float deltaTime)
        {
            _velocityY += GRAVITY * deltaTime;
            
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            
            Vector3 direction = new Vector3(x, 0, y);
            
            direction.Normalize();
            direction = transform.TransformDirection(direction);

            if (Input.GetButtonDown("Jump") && _controller.isGrounded) _velocityY = _jumpPower;

            float moveSpeed = _moveSpeed;

            if (Input.GetKeyDown(KeyCode.LeftShift)) _isSprinting = true;
            if (Input.GetKeyUp(KeyCode.LeftShift)) _isSprinting = false;

            if (_isSprinting) moveSpeed *= _sprintMultiplier;
            
            direction.y = _velocityY;
            
            _controller.Move(direction * (moveSpeed * deltaTime));
        }
    }
}