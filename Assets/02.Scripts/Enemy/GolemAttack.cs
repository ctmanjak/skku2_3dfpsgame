using Core;
using DG.Tweening;
using UnityEngine;

namespace Enemy
{
    public class GolemAttack : MonoBehaviour
    {
        private Transform _target;
        private Vector3 _targetPosition;

        [SerializeField] private GameObject _attackArea;
        [SerializeField] private Transform _fillAttackArea;
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private float _turnDuration = 0.5667f;

        private float _damage;
        private float _radius;
        private float _knockbackPower;

        private bool _isAttacking;

        public bool IsAttacking => _isAttacking;

        private void Awake()
        {
            _attackArea.SetActive(false);
        }

        public void Initialize(Transform target, float damage, float radius, float knockbackPower)
        {
            _target = target;
            _damage = damage;
            _radius = radius;
            _knockbackPower = knockbackPower;
        }

        public void AttackTelegraph(float readyDuration)
        {
            _isAttacking = true;
            
            _targetPosition = _target.position;

            Vector3 look = _targetPosition - transform.position;
            look.y = 0f;

            transform.parent.DORotateQuaternion(Quaternion.LookRotation(look.normalized, Vector3.up), _turnDuration);
            
            _attackArea.SetActive(true);
            _attackArea.transform.position = _targetPosition;
            _fillAttackArea.DOScale(Vector3.one, readyDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _fillAttackArea.localScale = Vector3.zero;
                    _attackArea.SetActive(false);
                });
        }
        
        public void Attack()
        {
            Collider[] colliders = Physics.OverlapSphere(_targetPosition, _radius, _enemyLayer);
            foreach (var col in colliders)
            {
                if (col.gameObject == gameObject) continue;
                
                IDamageable damageable = col.GetComponent<IDamageable>();
                damageable?.TakeDamage(new AttackContext(_damage));
                
                Vector3 knockbackDirection = (col.transform.position - _targetPosition).normalized;
                damageable?.Knockback(knockbackDirection * _knockbackPower);
            }
            
            _isAttacking = false;
        }
    }
}
