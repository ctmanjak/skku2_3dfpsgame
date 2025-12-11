using System.Collections;
using Core;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyMove))]
    [RequireComponent(typeof(EnemyStat))]
    [RequireComponent(typeof(EnemyRotate))]
    public class EnemyEntity : MonoBehaviour, IDamageable, IKnockbackable
    {
        public EEnemyState State = EEnemyState.Idle;

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
        
        private EnemyMove _enemyMove;
        private EnemyStat _enemyStat;
        private EnemyRotate _enemyRotate;

        private float _attackTimer;
        private Vector3 _patrolPosition;
        private float _patrolTimer;

        private void Awake()
        {
            _enemyMove = GetComponent<EnemyMove>();
            _enemyStat = GetComponent<EnemyStat>();
        }

        private void Start()
        {
            _originPosition = transform.position;
            _enemyMove.Initialize(_enemyStat.MoveSpeed.Value);
        }

        private void Update()
        {
            Vector3 directionToTarget = _target.position - transform.position;
            float distanceToTarget = directionToTarget.magnitude;
            
            if (distanceToTarget < _detectionDistance)
            {
                State = EEnemyState.Trace;
            }
            
            switch (State)
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
                State = EEnemyState.Comeback;
                return;
            }

            if (_patrolTimer > 0f) _patrolTimer -= Time.deltaTime;
            if (_patrolTimer <= 0f)
            {
                _patrolPosition = GetRandomPositionForPatrol();
                State = EEnemyState.Patrol;
                return;
            }
        }

        private void Trace()
        {
            Vector3 directionToTarget = _target.position - transform.position;
            float distanceToTarget = directionToTarget.magnitude;
            
            directionToTarget.y = 0f;
            _enemyMove.Move(directionToTarget.normalized, Time.deltaTime);

            if (distanceToTarget > _detectionDistance)
            {
                State = EEnemyState.Idle;
                return;
            }
            if (distanceToTarget <= _enemyStat.AttackDistance.Value)
            {
                State = EEnemyState.Attack;
                return;
            }
        }

        private void Comeback()
        {
            _enemyMove.Move((_originPosition - transform.position).normalized, Time.deltaTime);
            
            Vector3 directionToOrigin = _originPosition - transform.position;
            float distanceToOrigin = directionToOrigin.magnitude;
            
            if (distanceToOrigin <= _comebackRadius)
            {
                State = EEnemyState.Idle;
            }
        }

        private void Attack()
        {
            float distance = (_target.position - transform.position).magnitude;
            if (distance > _enemyStat.AttackDistance.Value)
            {
                State = EEnemyState.Idle;
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
            State = EEnemyState.Idle;
        }

        private IEnumerator DeathCoroutine()
        {
            yield return new WaitForSeconds(_deathDelay);
            Destroy(gameObject);
        }

        public void TakeDamage(float damage)
        {
            _enemyStat.Health.TryDecrease(damage);

            if (_enemyStat.Health.Value > 0)
            {
                State = EEnemyState.Hit;
                StartCoroutine(HitCoroutine());
            }
            else
            {
                State = EEnemyState.Death;
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
            
            directionToPatrol.y = 0f;
            _enemyMove.Move(directionToPatrol.normalized, Time.deltaTime);

            if (distanceToPatrol <= _patrolArriveRadius)
            {
                _patrolTimer = _patrolNextTime;
                State = EEnemyState.Idle;
            }
        }

        private Vector3 GetRandomPositionForPatrol()
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            float distance = Random.Range(_patrolMinRadius, _patrolMaxRadius);

            Vector3 offset = new Vector3(direction.x, 0f, direction.y) * distance;

            return _originPosition + offset;
        }
    }
}
