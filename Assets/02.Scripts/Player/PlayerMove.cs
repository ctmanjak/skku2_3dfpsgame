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
        private bool _canJump;
        private int _jumpCount;

        public int JumpCount => _jumpCount;

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

        public void Move(Vector2 axis, float deltaTime)
        {
            _velocityY += GRAVITY * deltaTime;
            
            Vector3 direction = new Vector3(axis.x, 0, axis.y);
            
            direction.Normalize();
            direction = transform.TransformDirection(direction);

            if (Input.GetButtonDown("Jump"))
            {
                if (_controller.isGrounded) Jump();
                
            }

            float moveSpeed = _moveSpeed * _sprintMultiplier;
            
            direction.y = _velocityY;
            
            _controller.Move(direction * (moveSpeed * deltaTime));
        }

        public void SetSprintMultiplier(float multiplier)
        {
            _sprintMultiplier = multiplier;
        }

        public void Jump()
        {
            _velocityY = _jumpPower;
            _jumpCount++;
        }

        public bool IsGrounded()
        {
            return _controller.isGrounded;
        }

        public void Grounding()
        {
            _jumpCount = 0;
        }
    }
}