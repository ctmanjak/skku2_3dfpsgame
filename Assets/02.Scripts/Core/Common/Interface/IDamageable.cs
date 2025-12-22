using UnityEngine;

namespace Core
{
    public interface IDamageable
    {
        public void TakeDamage(AttackContext context);
        public void Knockback(Vector3 direction);
    }
}