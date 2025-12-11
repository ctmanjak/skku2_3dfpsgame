using System;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(CharacterController))]
    public class EnemyMove : MonoBehaviour
    {
        private const float GRAVITY = -9.81f;

        [SerializeField] private float _extraPowerDuration = 0.3f;
        
        private CharacterController _controller;

        private float _moveSpeed; 
        private float _velocityY;
        private Vector3 _extraPower;
        private Vector3 _moveDirection;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        public void Initialize(float moveSpeed)
        {
            _moveSpeed = moveSpeed;
        }

        public void SetMoveDirection(Vector3 direction)
        {
            _moveDirection = direction;
        }

        public void Update()
        {
            float deltaTime = Time.deltaTime;
            float t = deltaTime / _extraPowerDuration;
            _extraPower = Vector3.Lerp(_extraPower, Vector3.zero, t);
            
            if (_controller.isGrounded && _velocityY < 0)
            {
                _velocityY = -1f;
            }
            
            _velocityY += GRAVITY * deltaTime;
            
            _moveDirection.Normalize();

            float moveSpeed = _moveSpeed;
            _moveDirection.y = _velocityY;

            Vector3 motion = _moveDirection * moveSpeed;
            _controller.Move((motion + _extraPower) * deltaTime);
        }

        public void AddExtraPower(Vector3 direction)
        {
            _extraPower += direction;
        }
    }
}