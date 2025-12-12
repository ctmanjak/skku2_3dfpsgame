using System.Collections;
using Core;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Enemy
{
    [RequireComponent(typeof(EnemyMove))]
    [RequireComponent(typeof(EnemyStat))]
    [RequireComponent(typeof(EnemyRotate))]
    public class EnemyEntity : MonoBehaviour, IDamageable
    {
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
        
        private EnemyMove _enemyMove;
        private EnemyStat _enemyStat;
        private EnemyRotate _enemyRotate;

        private float _attackTimer;
        private Vector3 _patrolPosition;
        private float _patrolTimer;

        private float _aggroTimer;

        private void Awake()
        {
            _enemyMove = GetComponent<EnemyMove>();
            _enemyStat = GetComponent<EnemyStat>();
            _enemyRotate = GetComponent<EnemyRotate>();
        }

        private void Start()
        {
            _originPosition = transform.position;
            _enemyMove.Initialize(_enemyStat.MoveSpeed.Value);
        }

        private void Update()
        {
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
            
            Move(directionToTarget);

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
            Move((_originPosition - transform.position));
            
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

            _attackTimer += Time.deltaTime;
            if (_attackTimer > _enemyStat.AttackSpeed.Value)
            {
                _attackTimer = 0f;
                
                IDamageable damageable = _target.GetComponent<IDamageable>();
                damageable.TakeDamage(_enemyStat.AttackDamage.Value);
            }
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

        public void TakeDamage(float damage)
        {
            _enemyStat.Health.Decrease(damage);

            if (_enemyStat.Health.Value > 0)
            {
                _aggroTimer = _aggroDuration;
                _state = EEnemyState.Hit;
                StartCoroutine(HitCoroutine());
            }
            else
            {
                _state = EEnemyState.Death;
                StartCoroutine(DeathCoroutine());
            }
        }

        public void Knockback(Vector3 direction)
        {
            _enemyMove.AddExtraPower(direction);
        }

        private void Patrol()
        {
            Vector3 directionToPatrol = _patrolPosition - transform.position;
            float distanceToPatrol = directionToPatrol.magnitude;
            
            Move(directionToPatrol);

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

        private void Move(Vector3 direction)
        {
            _enemyMove.SetMoveDirection(direction.normalized);

            Vector3 lookDirection = direction.normalized;
            lookDirection.y = 0;
            _enemyRotate.Rotate(lookDirection);
        }
    }
}
