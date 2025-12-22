using System.Collections;
using Core;
using UnityEditor.Animations;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Enemy
{
    [RequireComponent(typeof(EnemyStat))]
    [RequireComponent(typeof(EnemyRotate))]
    public class EnemyEntity : MonoBehaviour, IDamageable
    {
        private static readonly int _stateParam = Animator.StringToHash("State");

        [SerializeField] private Animator _animator;
        
        [SerializeField] private EEnemyState _state = EEnemyState.Idle;

        [SerializeField] private Vector3 _originPosition;
        [SerializeField] private Transform _target;
        [SerializeField] private float _detectionDistance = 3f;
        [SerializeField] private float _comebackRadius = 5f;
        [SerializeField] private float _hitDelay = 0.3f;
        [SerializeField] private float _deathDelay = 1f;

        [SerializeField] private float _patrolMinRadius = 3f;
        [SerializeField] private float _patrolMaxRadius = 6f;
        [SerializeField] private float _patrolArriveRadius = 0.5f;
        [SerializeField] private float _patrolNextTime = 1f;

        [SerializeField] private float _aggroDuration = 10f;

        [SerializeField] private GameObject _bloodSprayPrefab;
        
        [SerializeField] private GameObject _goldPrefab;
        [SerializeField] private Transform _goldDropAnchor;   // 적 몸(가슴/허리) 쪽에 빈 오브젝트로 만들고 할당
        [SerializeField] private float _goldDropRadius = 0.6f; // XZ 퍼짐 반경(너무 크면 어색)
        [SerializeField] private float _goldEjectForce = 3.5f; // 튀어나오는 힘(프리팹 Rigidbody 있을 때만)
        [SerializeField] private int _goldDropCount = 5;          // 고정 개수
        [SerializeField] private float _goldMinAboveAnchorY = 0.1f; // anchor보다 최소 얼마 위에서 스폰할지
        [SerializeField] private float _goldExtraAboveAnchorY = 0.3f; // 추가로 랜덤하게 더 올릴 범위(0이면 고정)
        [SerializeField] private float _goldEjectConeAngle = 35f; // 원뿔 반각(도). 20~45 정도가 보통 자연스러움
        [SerializeField] private float _goldEjectUpBias = 0.8f;   // 위쪽 축 성분(0~1). 클수록 더 위로 쏠림
        
        private bool _goldDropped;
        
        private EnemyMove _enemyMove;
        private EnemyStat _enemyStat;
        private EnemyRotate _enemyRotate;
        private EnemyAttack _enemyAttack;

        private float _attackTimer;
        private Vector3 _patrolPosition;
        private float _patrolTimer;

        private float _aggroTimer;

        private void Awake()
        {
            _enemyMove = GetComponentInChildren<EnemyMove>();
            _enemyStat = GetComponent<EnemyStat>();
            _enemyRotate = GetComponent<EnemyRotate>();
            _enemyAttack = GetComponentInChildren<EnemyAttack>();
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }
        }

        private void Start()
        {
            _originPosition = transform.position;
            _enemyMove.Initialize(_enemyStat.MoveSpeed.Value, _enemyStat.JumpPower.Value);
        }

        private void Update()
        {
            if (_state == EEnemyState.Death) return;
            
            if (_state != EEnemyState.Attack)
            {
                Vector3 directionToTarget = _target.position - transform.position;
                float distanceToTarget = directionToTarget.magnitude;

                _aggroTimer -= Time.deltaTime;

                if (_aggroTimer > 0f || distanceToTarget < _detectionDistance)
                {
                    _state = EEnemyState.Trace;
                }
            }
            
            switch (_state)
            {
                case EEnemyState.Idle:
                    Idle();
                    break;
                
                case EEnemyState.Trace:
                    Trace();
                    break;
                
                case EEnemyState.Comeback:
                    Comeback();
                    break;
                
                case EEnemyState.Attack:
                    Attack();
                    break;
                
                case EEnemyState.Patrol:
                    Patrol();
                    break;
            }
            
            _animator.SetInteger(_stateParam, (int)_state);
        }

        private void Idle()
        {
            Vector3 directionToOrigin = _originPosition - transform.position;
            float distanceToOrigin = directionToOrigin.magnitude;
            
            if (distanceToOrigin > _patrolMaxRadius)
            {
                _state = EEnemyState.Comeback;
                return;
            }

            if (_patrolTimer > 0f) _patrolTimer -= Time.deltaTime;
            if (_patrolTimer <= 0f)
            {
                _patrolPosition = GetRandomPositionForPatrol();
                _state = EEnemyState.Patrol;
                return;
            }
        }

        private void Trace()
        {
            Vector3 directionToTarget = _target.position - transform.position;
            float distanceToTarget = directionToTarget.magnitude;
            
            Move(_target.position);

            if (distanceToTarget > _detectionDistance)
            {
                _state = EEnemyState.Idle;
                return;
            }
            if (distanceToTarget <= _enemyStat.AttackDistance.Value)
            {
                _state = EEnemyState.Attack;
                return;
            }
        }

        private void Comeback()
        {
            Move(_originPosition);
            
            Vector3 directionToOrigin = _originPosition - transform.position;
            float distanceToOrigin = directionToOrigin.magnitude;
            
            if (distanceToOrigin <= _comebackRadius)
            {
                _state = EEnemyState.Idle;
            }
        }

        private void Attack()
        {
            float distance = (_target.position - transform.position).magnitude;
            if (distance > _enemyStat.AttackDistance.Value)
            {
                _state = EEnemyState.Idle;
                return;
            }
            
            _enemyAttack.SetTarget(_target);
            _enemyAttack.SetDamage(_enemyStat.AttackDamage.Value);
        }

        private IEnumerator HitCoroutine()
        {
            yield return new WaitForSeconds(_hitDelay);
            _state = EEnemyState.Idle;
        }

        private IEnumerator DeathCoroutine()
        {
            yield return new WaitForSeconds(_deathDelay);
            Destroy(gameObject);
        }
        
        private static Vector3 RandomDirectionInCone(Vector3 axis, float halfAngleDeg)
        {
            axis = axis.normalized;

            // 구면캡에서 균일 샘플링: cos(theta)를 [cos(a), 1]에서 균일하게 뽑음
            float halfAngleRad = halfAngleDeg * Mathf.Deg2Rad;
            float cosMin = Mathf.Cos(halfAngleRad);
            float u = Random.value;
            float v = Random.value;

            float cosTheta = Mathf.Lerp(cosMin, 1f, u);
            float sinTheta = Mathf.Sqrt(1f - cosTheta * cosTheta);
            float phi = 2f * Mathf.PI * v;

            // 로컬(Z축 기준) 방향 벡터
            Vector3 localDir = new Vector3(
                sinTheta * Mathf.Cos(phi),
                sinTheta * Mathf.Sin(phi),
                cosTheta
            );

            // localDir의 +Z를 axis로 회전시켜 월드 방향으로 변환
            return Quaternion.FromToRotation(Vector3.forward, axis) * localDir;
        }

        private Vector3 GetGoldEjectAxis()
        {
            // “위쪽으로 퍼지는” 축을 만들되, 살짝 전방으로도 기울 수 있게
            Vector3 axis = (Vector3.up * _goldEjectUpBias + transform.forward * (1f - _goldEjectUpBias));
            if (axis.sqrMagnitude < 0.0001f) axis = Vector3.up;
            return axis.normalized;
        }
        
        public void DropGold()
        {
            if (_goldDropped) return;
            _goldDropped = true;

            int count = Mathf.Max(0, _goldDropCount);

            Vector3 origin = _goldDropAnchor ? _goldDropAnchor.position : transform.position;

            for (int c = 0; c < count; c++)
            {
                Vector2 rnd = Random.insideUnitCircle * _goldDropRadius;
                Vector3 offset =
                    transform.right * rnd.x +
                    transform.forward * rnd.y;   // Y는 여기서 안 만듦

                float extraY = _goldMinAboveAnchorY + Random.Range(0f, _goldExtraAboveAnchorY);
                Vector3 candidate = origin + offset;
                candidate.y = Mathf.Max(candidate.y, origin.y + extraY); // 무조건 anchor보다 높게

                GameObject goldObject = PoolManager.Get(_goldPrefab);
                goldObject.transform.position = candidate;

                // 튀어나오는 연출(있으면 적용)
                Rigidbody rb = goldObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;

                    Vector3 axis = GetGoldEjectAxis();
                    Vector3 ejectDir = RandomDirectionInCone(axis, _goldEjectConeAngle);

                    // 안전장치: “위쪽으로 퍼지는” 요구가 강하면 아래로 향하는 성분은 제거
                    if (ejectDir.y < 0f) ejectDir.y = 0.05f;
                    ejectDir.Normalize();

                    // 개별 코인마다 힘을 조금 랜덤
                    float force = _goldEjectForce * Random.Range(0.8f, 1.2f);
                    rb.AddForce(ejectDir * force, ForceMode.VelocityChange);
                }
            }
        }

        public void TakeDamage(AttackContext context)
        {
            _enemyStat.Health.Decrease(context.Damage);
            
            if (context.HitPoint != null && context.HitNormal != null)
            {
                Instantiate(_bloodSprayPrefab, (Vector3)context.HitPoint, Quaternion.LookRotation((Vector3)context.HitNormal), transform);
            }

            if (_enemyStat.Health.Value > 0)
            {
                _aggroTimer = _aggroDuration;
                _state = EEnemyState.Hit;
                StartCoroutine(HitCoroutine());
            }
            else
            {
                _state = EEnemyState.Death;
                DropGold();
                StartCoroutine(DeathCoroutine());
            }
            _animator.SetInteger(_stateParam, (int)_state);
        }

        public void Knockback(Vector3 direction)
        {
            _enemyMove.AddExtraPower(direction);
        }

        private void Patrol()
        {
            Vector3 directionToPatrol = _patrolPosition - transform.position;
            float distanceToPatrol = directionToPatrol.magnitude;
            
            Move(_patrolPosition);

            if (distanceToPatrol <= _patrolArriveRadius)
            {
                _patrolTimer = _patrolNextTime;
                _state = EEnemyState.Idle;
            }
        }

        private Vector3 GetRandomPositionForPatrol()
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            float distance = Random.Range(_patrolMinRadius, _patrolMaxRadius);

            Vector3 offset = new Vector3(direction.x, 0f, direction.y) * distance;

            return _originPosition + offset;
        }

        private void Move(Vector3 destination)
        {
            _enemyMove.SetDestination(destination);
        }
    }
}
