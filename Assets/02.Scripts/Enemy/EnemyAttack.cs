using Core;
using UnityEngine;

namespace Enemy
{
    public class EnemyAttack : MonoBehaviour
    {
        private Transform _target;

        private float _damage;

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void SetDamage(float damage)
        {
            _damage = damage;
        }
        
        public void Attack()
        {
            IDamageable damageable = _target.GetComponent<IDamageable>();
            damageable.TakeDamage(_damage);
        }
    }
}
