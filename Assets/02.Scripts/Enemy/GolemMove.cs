using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class GolemMove : MonoBehaviour
    {
        private static readonly int _jump = Animator.StringToHash("Jump");
        
        private const float GRAVITY = -9.81f;

        [SerializeField] private float _extraPowerDuration = 0.3f;
        [SerializeField] private float _jumpDesiredDistance = 1.5f;
        [SerializeField] private float _ignoreExtraPowerValue = 0.01f;
        [SerializeField] private float _jumpTurnSpeed = 720f;
        
        private CharacterController _controller;

        private NavMeshAgent _agent;
        private Animator _animator;

        private float _jumpPower;
        private float _moveSpeed; 
        private float _velocityY;
        private Vector3 _extraPower;
        private Vector3 _moveDirection;
        private Transform _target;
        private bool _isStop;
        
        // Cinematic Parabola Jump
        private bool _isParabolaJumping;
        private float _jumpStartTime;
        private float _jumpDuration;
        private Vector3 _jumpStart;
        private Vector3 _jumpEnd;
        private float _jumpHeight;
        

        private bool _savedDetectCollisions;

        private OffMeshLinkData? _currentLinkData;

        private void Awake()
        {
            _controller = GetComponentInParent<CharacterController>();
            _agent = GetComponentInParent<NavMeshAgent>();

            _animator = GetComponent<Animator>();
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

        public void Freeze()
        {
            _isStop = true;
        }

        public void Unfreeze()
        {
            _isStop = false;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            
            if (_isParabolaJumping)
            {
                TickParabolaJump();
                return;
            }
            
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
                    _animator.SetTrigger(_jump);
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
            if (_isStop)
            {
                motion.x = 0f;
                motion.z = 0f;
            }
            _controller.Move((motion + _extraPower) * deltaTime);
        }

        public void Jump()
        {
            _velocityY = _jumpPower;
        }

        public void AddExtraPower(Vector3 direction)
        {
            _extraPower += direction;
        }

        public void InitParabolaJump(Vector3 endPos, float duration)
        {
            if (_isParabolaJumping) return;
            
            _jumpEnd = endPos;
            _jumpDuration = Mathf.Max(0.01f, duration);
        }
        
        public void StartParabolaJump()
        {
            if (_isParabolaJumping) return;

            _isParabolaJumping = true;

            _jumpStart = transform.position;
            _jumpStartTime = Time.time;

            // 높이 자동 계산(연출용 휴리스틱)
            // - 수평 거리 기반 기본 높이
            // - 목표 지점이 더 높으면 약간 추가
            Vector3 flat = new Vector3(_jumpEnd.x - _jumpStart.x, 0f, _jumpEnd.z - _jumpStart.z);
            float flatDist = flat.magnitude;
            float upDiff = Mathf.Max(0f, _jumpEnd.y - _jumpStart.y);

            _jumpHeight = Mathf.Max(0.25f, flatDist * 0.35f) + upDiff * 0.25f;

            // 기존 이동 영향 제거
            _velocityY = 0f;
            _extraPower = Vector3.zero;
            _currentLinkData = null;

            // 충돌 무시(연출 우선)
            _savedDetectCollisions = _controller.detectCollisions;
            _controller.detectCollisions = false;

            // 네비 내부 상태 튐 방지
            _agent.Warp(transform.position);
            _agent.nextPosition = transform.position;
        }
        
        private void TickParabolaJump()
        {
            float elapsed = Time.time - _jumpStartTime;

            if (elapsed >= _jumpDuration)
            {
                // duration에 정확히 착지
                Vector3 snapDelta = _jumpEnd - transform.position;
                _controller.Move(snapDelta);

                // 상태 복구
                _controller.detectCollisions = _savedDetectCollisions;
                _isParabolaJumping = false;

                _velocityY = -1f;
                _agent.Warp(transform.position);
                _agent.nextPosition = transform.position;
                return;
            }

            float t = Mathf.Clamp01(elapsed / _jumpDuration);

            // 선형 보간 위치 + 포물선 y 오프셋(정점은 t=0.5)
            Vector3 basePos = Vector3.Lerp(_jumpStart, _jumpEnd, t);
            float yOffset = 4f * _jumpHeight * t * (1f - t);

            Vector3 desiredPos = basePos + Vector3.up * yOffset;

            // CharacterController는 delta로 이동
            Vector3 delta = desiredPos - transform.position;
            _controller.Move(delta);

            _agent.nextPosition = transform.position;
        }
    }
}