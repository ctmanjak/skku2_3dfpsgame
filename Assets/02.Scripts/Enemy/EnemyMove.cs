using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(CharacterController))]
    public class EnemyMove : MonoBehaviour
    {
        private const float GRAVITY = -9.81f;

        [SerializeField] private float _extraPowerDuration = 0.3f;
        [SerializeField] private float _jumpDesiredDistance = 1.5f;
        [SerializeField] private float _ignoreExtraPowerValue = 0.01f;
        
        private CharacterController _controller;

        private NavMeshAgent _agent;

        private float _jumpPower;
        private float _moveSpeed; 
        private float _velocityY;
        private Vector3 _extraPower;
        private Vector3 _moveDirection;
        private Transform _target;

        private OffMeshLinkData? _currentLinkData;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _agent = GetComponent<NavMeshAgent>();
        }

        public void Initialize(float moveSpeed, float jumpPower)
        {
            _moveSpeed = moveSpeed;
            _jumpPower = jumpPower;
            _agent.updatePosition = false;
        }

        public void SetDestination(Vector3 destination)
        {
            _agent.SetDestination(destination);
        }

        public Vector3 GetSteeringTarget()
        {
            return _agent.steeringTarget;
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
            
            float moveSpeed = _moveSpeed;
            
            if (_agent.isOnOffMeshLink)
            {
                _currentLinkData = _agent.currentOffMeshLinkData;
            }

            if (_extraPower.sqrMagnitude < _ignoreExtraPowerValue && _extraPower != Vector3.zero)
            {
                _extraPower = Vector3.zero;
                _agent.Warp(transform.position);
                _currentLinkData = null;
            }
            
            if (_currentLinkData != null)
            {
                Vector3 toStartPos = _currentLinkData.Value.startPos - transform.position;
                
                if (toStartPos.magnitude <= _jumpDesiredDistance)
                {
                    _agent.SetDestination(_currentLinkData.Value.endPos);
                    Jump();
                    _agent.CompleteOffMeshLink();
                    _currentLinkData = null;
                }
                else
                {
                    _agent.SetDestination(_currentLinkData.Value.startPos);
                }
            }
            
            Vector3 moveDirection = (_agent.nextPosition - transform.position).normalized;

            moveDirection.y = _velocityY;
            Vector3 motion = moveDirection * moveSpeed;
            _controller.Move((motion + _extraPower) * deltaTime);
        }

        private void Jump()
        {
            _velocityY = _jumpPower;
        }

        public void AddExtraPower(Vector3 direction)
        {
            _extraPower += direction;
        }
    }
}