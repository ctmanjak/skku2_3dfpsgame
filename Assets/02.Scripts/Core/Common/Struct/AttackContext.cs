using UnityEngine;

namespace Core
{
    public struct AttackContext
    {
        public float Damage;
        public Vector3? HitPoint;
        public Vector3? HitNormal;

        public AttackContext(float damage)
        {
            Damage = damage;
            HitPoint = null;
            HitNormal = null;
        }
        
        public AttackContext(float damage, Vector3 hitPoint)
        {
            Damage = damage;
            HitPoint = hitPoint;
            HitNormal = null;
        }
        
        public AttackContext(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            Damage = damage;
            HitPoint = hitPoint;
            HitNormal = hitNormal;
        }
    }
}