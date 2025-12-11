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
        private float _extraPowerTimer;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        public void Initialize(float moveSpeed)
        {
            _moveSpeed = moveSpeed;
        }

        public void Move(Vector3 direction, float deltaTime)
        {
            _velocityY += GRAVITY * deltaTime;
            
            direction.Normalize();

            float moveSpeed = _moveSpeed;
            direction.y = _velocityY;

            Vector3 motion = direction * (moveSpeed * deltaTime);
            _controller.Move(motion);
        }

        private void Update()
        {
            if (_extraPowerTimer > 0f) _extraPowerTimer -= Time.deltaTime;
            _extraPower = Vector3.Lerp(_extraPower, Vector3.zero, (_extraPowerDuration - _extraPowerTimer) / _extraPowerDuration);
            
            if (_extraPower != Vector3.zero) _controller.Move(_extraPower * Time.deltaTime);
        }

        public void AddExtraPower(Vector3 direction)
        {
            _extraPowerTimer = _extraPowerDuration;
            _extraPower += direction;
        }
    }
}