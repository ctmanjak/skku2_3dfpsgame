using Core;
using UnityEngine;

namespace Gold
{
    public class GoldPickupMagnet : MonoBehaviour
    {
        [Header("Repel (뒤로 살짝)")]
        [SerializeField] private float _repelDuration = 0.08f;
        [SerializeField] private float _repelDistance = 0.35f;

        [Header("Attract (흡수)")]
        [SerializeField] private float _attractAcceleration = 55f; // units/s^2
        [SerializeField] private float _maxSpeed = 18f;
        [SerializeField] private float _collectRadius = 0.7f;
        [SerializeField] private float _hardSnapRadius = 0.8f;   // 이 안으로 들어오면 강제로 붙여서 수집(무조건 먹힘)

        [Header("Pooling")]
        [SerializeField] private bool _disableOnCollect = true;
        

        private Rigidbody _rb;
        private Collider _collider;
        private Transform _target;

        private bool _isMagneting;
        private float _timer;

        private Vector3 _repelStartPos;
        private Vector3 _repelEndPos;
        private Vector3 _velocity;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        private void OnEnable()
        {
            _isMagneting = false;
            _target = null;
            _timer = 0f;
            _velocity = Vector3.zero;
        }

        public void BeginMagnet(Transform collectPoint)
        {
            if (_isMagneting) return;
            if (collectPoint == null) return;

            _isMagneting = true;
            _target = collectPoint;

            _timer = 0f;
            _velocity = Vector3.zero;

            _repelStartPos = transform.position;

            // “뒤로” = (골드 위치 - 플레이어 위치) 방향 (플레이어로부터 멀어지는 방향)
            Vector3 awayDir = (_repelStartPos - collectPoint.position);
            if (awayDir.sqrMagnitude < 0.0001f) awayDir = Vector3.up;
            awayDir.y = 0f;
            awayDir.Normalize();

            _repelEndPos = _repelStartPos + awayDir * _repelDistance;

            // 물리 영향 끄고(튀는/굴러가는 중이라면) 흡수 연출은 우리가 직접 제어
            if (_rb != null)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
                _rb.isKinematic = true;
            }

            if (_collider != null)
            {
                _collider.isTrigger = true;
            }
        }

        private void Update()
        {
            if (!_isMagneting || _target == null) return;

            // 1) 잠깐 뒤로
            if (_timer < _repelDuration)
            {
                float t = (_repelDuration <= 0f) ? 1f : (_timer / _repelDuration);
                // 살짝 “튀기는” 느낌: SmoothStep
                float s = Mathf.SmoothStep(0f, 1f, t);

                transform.position = Vector3.LerpUnclamped(_repelStartPos, _repelEndPos, s);

                _timer += Time.deltaTime;
                return;
            }

            
            // 2) 플레이어로 흡수
            Vector3 targetPos = _target.position;
            Vector3 currPos = transform.position;

            Vector3 toTarget = targetPos - currPos;
            float dist = toTarget.magnitude;

            // (C) 최종 안전장치: 충분히 가까우면 스냅 후 수집
            if (dist <= _hardSnapRadius)
            {
                transform.position = targetPos;
                Collect();
                return;
            }

            Vector3 dir = (dist > 0.0001f) ? (toTarget / dist) : Vector3.up;

            // (A) 옆방향 속도 제거: 속도는 항상 타겟 방향 성분만 남김
            float forwardSpeed = Mathf.Max(0f, Vector3.Dot(_velocity, dir));
            _velocity = dir * forwardSpeed;

            // 가속
            _velocity += dir * _attractAcceleration * Time.deltaTime;

            // 속도 제한
            float speed = _velocity.magnitude;
            if (speed > _maxSpeed) _velocity = _velocity / speed * _maxSpeed;

            // 다음 위치
            Vector3 nextPos = currPos + _velocity * Time.deltaTime;

            // (B) 프레임 사이에 수집 반경을 “통과”하면 무조건 수집
            if (DistancePointToSegment(targetPos, currPos, nextPos) <= _collectRadius)
            {
                transform.position = targetPos;
                Collect();
                return;
            }

            transform.position = nextPos;
        }
        
        private static float DistancePointToSegment(Vector3 p, Vector3 a, Vector3 b)
        {
            Vector3 ab = b - a;
            float abSqr = ab.sqrMagnitude;
            if (abSqr < 0.000001f) return Vector3.Distance(p, a);

            float t = Vector3.Dot(p - a, ab) / abSqr;
            t = Mathf.Clamp01(t);
            Vector3 closest = a + ab * t;
            return Vector3.Distance(p, closest);
        }

        private void Collect()
        {
            _isMagneting = false;
            _target = null;
            _velocity = Vector3.zero;

            // TODO: 여기서 골드 증가 처리(예: GoldManager.Add(1))를 넣으세요.

            if (_rb != null) _rb.isKinematic = false;
            if (_collider != null) _collider.isTrigger = false;

            if (_disableOnCollect)
            {
                // PoolManager를 쓰면 여기서 Return/Release로 바꿔도 됩니다.
                PoolManager.Release(gameObject);
            }
        }
    }
}