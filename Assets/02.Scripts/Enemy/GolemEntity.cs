using System.Collections;
using Core;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Enemy
{
    [RequireComponent(typeof(GolemStat))]
    [RequireComponent(typeof(EnemyRotate))]
    public class GolemEntity : MonoBehaviour, IDamageable
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
        
        private GolemMove _golemMove;
        private GolemStat _golemStat;
        private GolemAttack _golemAttack;

        private float _attackTimer;
        private Vector3 _patrolPosition;
        private float _patrolTimer;

        private float _aggroTimer;

        private void Awake()
        {
            _golemMove = GetComponentInChildren<GolemMove>();
            _golemStat = GetComponent<GolemStat>();
            _golemAttack = GetComponentInChildren<GolemAttack>();
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }
        }

        private void Start()
        {
            _originPosition = transform.position;
            _golemMove.Initialize(_golemStat.MoveSpeed.Value, _golemStat.JumpPower.Value);
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
            if (distanceToTarget <= _golemStat.AttackDistance.Value)
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
            if (distance > _golemStat.AttackDistance.Value)
            {
                _state = EEnemyState.Idle;
                return;
            }
            
            _golemAttack.Initialize(_target, _golemStat.AttackDamage.Value, _golemStat.AttackRadius.Value, _golemStat.AttackKnockbackPower.Value);
            _golemMove.InitParabolaJump(_target.position, _golemStat.AttackDelay.Value);
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

        public void TakeDamage(AttackContext context)
        {
            _golemStat.Health.Decrease(context.Damage);

            if (_golemStat.Health.Value > 0)
            {
                _aggroTimer = _aggroDuration;

                if (!_golemAttack.IsAttacking)
                {
                    _state = EEnemyState.Hit;
                    StartCoroutine(HitCoroutine());
                }
            }
            else
            {
                _state = EEnemyState.Death;
                StartCoroutine(DeathCoroutine());
            }
            _animator.SetInteger(_stateParam, (int)_state);
        }

        public void Knockback(Vector3 direction)
        {
            _golemMove.AddExtraPower(direction);
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
            _golemMove.SetDestination(destination);
        }
    }
}
