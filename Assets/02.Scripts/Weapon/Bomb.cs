using System;
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
        [SerializeField] private float _minKnockbackPowerY = 0.5f;

        [SerializeField] private Rigidbody _rigidbody;
        
        [SerializeField] private LayerMask _enemyLayer;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius, _enemyLayer);
            foreach (var hit in hits)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                damageable?.TakeDamage(new AttackContext(_damage));

                Vector3 knockbackDirection = (hit.transform.position - transform.position).normalized;
                knockbackDirection.y = Math.Max(_minKnockbackPowerY, knockbackDirection.y); 
                damageable?.Knockback(knockbackDirection * _knockbackPower);
            }
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            PoolManager.Release(gameObject);
        }
    }
}