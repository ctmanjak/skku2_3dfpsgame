using UnityEngine;

namespace Core
{
    public interface IKnockbackable
    {
        public void Knockback(Vector3 direction);
    }
}