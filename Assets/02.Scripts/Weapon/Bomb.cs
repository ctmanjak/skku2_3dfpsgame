using Core;
using UnityEngine;

namespace Weapon
{
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private GameObject _explosionEffectPrefab;
        [SerializeField] private float _damage = 50f;
        [SerializeField] private float _knockbackPower = 20f;
        [SerializeField] private float _radius = 5f;
        
        [SerializeField] private LayerMask _enemyLayer;
        
    
        private void OnCollisionEnter(Collision collision)
        {
            Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius, _enemyLayer);
            
            foreach (var hit in hits)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                damageable.TakeDamage(_damage);

                Vector3 knockbackDirection = (hit.transform.position - transform.position).normalized;
                
                IKnockbackable knockbackable = hit.GetComponent<IKnockbackable>();
                knockbackable.Knockback(knockbackDirection * _knockbackPower);
            }
            Destroy(gameObject);
        }
    }
}