using System.Collections;
using Core;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Drum
{
    [RequireComponent(typeof(DrumMove))]
    [RequireComponent(typeof(DrumStat))]
    public class DrumEntity : MonoBehaviour, IDamageable
    {
        [SerializeField] private GameObject _explosionPrefab;
        
        private DrumMove _drumMove;
        private DrumStat _drumStat;

        private Collider[] _explosionColliders = new Collider[16];

        private void Awake()
        {
            _drumMove = GetComponent<DrumMove>();
            _drumStat = GetComponent<DrumStat>();
        }

        private IEnumerator DeathCoroutine()
        {
            yield return new WaitForSeconds(_drumStat.ExplosionDelay.Value);
            Explosion();
            yield return new WaitForSeconds(_drumStat.DeathDelay.Value);
            Destroy(gameObject);
        }

        private void Explosion()
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _drumMove.AddForce(Vector3.up * _drumStat.ExplosionJumpForce.Value);
            _drumMove.AddTorque(transform.forward * _drumStat.ExplosionTorque.Value);
            Physics.OverlapSphereNonAlloc(transform.position, _drumStat.ExplosionRadius.Value, _explosionColliders);

            Collider thisCollider = GetComponent<Collider>();
            foreach (var explosionCollider in _explosionColliders)
            {
                if (explosionCollider == null || explosionCollider == thisCollider) continue;
                float distance = (explosionCollider.transform.position - transform.position).magnitude;
                float t = Mathf.Clamp01(1f - distance / _drumStat.ExplosionRadius.Value);
                
                IDamageable damageable = explosionCollider.GetComponent<IDamageable>();
                damageable?.TakeDamage(_drumStat.ExplosionDamage.Value * t);

                Vector3 knockbackDirection = (explosionCollider.transform.position - transform.position).normalized;
                damageable?.Knockback(knockbackDirection * (_drumStat.ExplosionKnockbackPower.Value * t));
            }
        }

        public void TakeDamage(float damage)
        {
            if (_drumStat.Health.Value <= 0f) return;
            
            _drumStat.Health.Decrease(damage);

            if (_drumStat.Health.Value <= 0f)
            {
                StartCoroutine(DeathCoroutine());
            }
        }

        public void Knockback(Vector3 force)
        {
            _drumMove.AddForce(force);
        }
    }
}
