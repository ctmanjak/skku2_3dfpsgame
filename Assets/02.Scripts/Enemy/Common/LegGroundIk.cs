using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Enemy
{
    public class LegGroundIk : MonoBehaviour
    {
        [Header("IK")]
        [SerializeField] private TwoBoneIKConstraint _ik;

        [Header("참조")]
        [SerializeField] private Transform _body;          // 거미 몸(기준)
        [SerializeField] private LayerMask _groundMask;

        [Header("앵커 설정")]
        [SerializeField] private Vector3 _localAnchor;     // 몸 기준 다리의 "이상적인" 위치
        [SerializeField] private Vector3 _localAnchorOffset;   // 발마다 따로 줄 오프셋
        [SerializeField] private bool _autoAnchorFromStart = true;
        [SerializeField] private float _raycastUp = 1f;
        [SerializeField] private float _raycastDown = 2f;
        [SerializeField] private float _footOffset = 0.02f; // 해결2에서 쓰던 오프셋

        [Header("스텝 조건")]
        [SerializeField] private float _extraForward = 0.3f; // 추가로 더 나갈 거리
        [SerializeField] private float _maxStepDistance = 0.7f;  // 이 거리 이상 벌어지면 스텝
        [SerializeField] private float _stepDuration = 0.15f;    // 한 번 디딜 때 걸리는 시간
        [SerializeField] private float _stepHeight = 0.1f;       // 스텝할 때 살짝 들어올리는 높이
        [SerializeField] private LegGroundIk[] _mustBeGroundedLegs;
        
        [Header("Leg Relations")]
        [SerializeField] private LegGroundIk _oppositeLeg; // 반대발
        [SerializeField] private float _minAhead = 0.3f;        // 반대발보다 최소 얼마나 앞에 둘지

        private Transform _target;       // IK 타겟
        private Vector3 _currentAnchor;  // 현재 발이 "박혀있는" 월드 위치
        private bool _hasAnchor;

        // 스텝 진행 상태
        private bool _isStepping;
        private float _stepT;
        private Vector3 _stepStartPos;
        private Vector3 _stepEndPos;
        
        public bool IsStepping => _isStepping;
        public bool IsGrounded => !_isStepping;
        public Vector3 CurrentAnchor => _currentAnchor;

        private void Awake()
        {
            _target = _ik.data.target;

            // 처음에 _localAnchor를 자동으로 잡고 싶으면 이렇게도 가능:
            _localAnchor = _body.InverseTransformPoint(_target.position);
        }

        private void Start()
        {
            // 시작할 때 한 번 현재 위치로 앵커 찍기
            UpdateAnchorFromGround(bodyBased: true);
            _target.position = _currentAnchor;
        }

        private void LateUpdate()
        {
            Vector3 desiredAnchor = _body.TransformPoint(_localAnchor + _localAnchorOffset);

            if (!_isStepping)
            {
                float dist = Vector3.Distance(_currentAnchor, desiredAnchor);

                // 🔥 여기 조건에 "다른 다리들이 모두 Grounded인지"를 추가
                if (dist > _maxStepDistance && CanStepNow())
                {
                    StartStep(desiredAnchor);
                }
                else
                {
                    _target.position = _currentAnchor;
                }
            }
            else
            {
                // 기존 스텝 보간 로직 그대로...
                _stepT += Time.deltaTime / _stepDuration;
                float t = Mathf.Clamp01(_stepT);

                Vector3 flat = Vector3.Lerp(_stepStartPos, _stepEndPos, t);
                float height = Mathf.Sin(t * Mathf.PI) * _stepHeight;
                Vector3 pos = flat + Vector3.up * height;

                _target.position = pos;

                if (t >= 1f)
                {
                    FinishStep();
                }
            }
        }

        private void StartStep(Vector3 desiredAnchor)
        {
            _isStepping = true;
            _stepT = 0f;
            _stepStartPos = _target.position;

            // 1. 기본 목표 위치 (몸 기준 앵커 + 너가 쓰던 extraForward 등)
            Vector3 basePos = desiredAnchor;

            // 몸의 이동/forward 방향 (수평)
            Vector3 moveDir = _body.forward;
            moveDir.y = 0f;
            if (moveDir.sqrMagnitude < 0.0001f)
                moveDir = Vector3.forward; // fallback
            moveDir.Normalize();

            // 2. 반대발보다 항상 조금 앞에 두기
            if (_oppositeLeg != null)
            {
                Vector3 oppositePos = _oppositeLeg.CurrentAnchor;

                // 몸 기준으로 각 발이 forward 방향으로 얼마나 나가 있는지 투영
                Vector3 bodyPos = _body.position;
                float thisAlong  = Vector3.Dot(basePos     - bodyPos, moveDir);
                float oppAlong   = Vector3.Dot(oppositePos - bodyPos, moveDir);

                float minThisAlong = oppAlong + _minAhead;
                if (thisAlong < minThisAlong)
                {
                    float delta = minThisAlong - thisAlong;
                    basePos += moveDir * delta; // forward 방향으로 더 밀어줌
                }
            }

            // 3. 최종적으로는 바닥에 투영해서 stepEnd로 사용
            _stepEndPos = GetGroundPoint(basePos);
        }

        private void FinishStep()
        {
            _isStepping = false;
            _currentAnchor = _stepEndPos; // 이제 이 위치가 새로운 "박힌 위치"
            _target.position = _currentAnchor;
        }
        
        private bool CanStepNow()
        {
            // 반대쪽/지정된 다리들이 모두 Grounded일 때만 true
            if (_mustBeGroundedLegs == null) return true;

            foreach (var leg in _mustBeGroundedLegs)
            {
                if (leg == null) continue;
                if (!leg.IsGrounded) return false;
            }

            return true;
        }

        private void UpdateAnchorFromGround(bool bodyBased)
        {
            Vector3 basePos = bodyBased
                ? _body.TransformPoint(_localAnchor + _localAnchorOffset)
                : _target.position;

            _currentAnchor = GetGroundPoint(basePos);
            _hasAnchor = true;
        }

        private Vector3 GetGroundPoint(Vector3 basePos)
        {
            Vector3 origin = basePos + Vector3.up * _raycastUp;
            Vector3 dir = Vector3.down;

            if (Physics.Raycast(origin, dir, out var hit, _raycastUp + _raycastDown, _groundMask))
            {
                // 해결2: hit.point에서 normal 방향으로 footOffset만큼 띄우기
                return hit.point + hit.normal * _footOffset;
            }

            // 땅을 못 찾으면 그냥 기존 앵커나 베이스 위치 사용 (보험)
            return _hasAnchor ? _currentAnchor : basePos;
        }
    }
}