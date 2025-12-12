using UnityEngine;

namespace Core
{
    public interface IDamageable
    {
        public void TakeDamage(float damage);
        public void Knockback(Vector3 direction);
    }
}