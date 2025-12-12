using Core;
using UnityEngine;

namespace Drum
{
    public class DrumStat : MonoBehaviour
    {
        [SerializeField] private ConsumableStat _health;
        
        [SerializeField] private ValueStat _deathDelay;
        
        [SerializeField] private ValueStat _explosionJumpForce;
        [SerializeField] private ValueStat _explosionTorque;
        [SerializeField] private ValueStat _explosionDelay;
        [SerializeField] private ValueStat _explosionRadius;
        [SerializeField] private ValueStat _explosionDamage;
        [SerializeField] private ValueStat _explosionKnockbackPower;

        public ConsumableStat Health => _health;

        public ValueStat DeathDelay => _deathDelay;
        public ValueStat ExplosionJumpForce => _explosionJumpForce;
        public ValueStat ExplosionTorque => _explosionTorque;
        public ValueStat ExplosionDelay => _explosionDelay;
        public ValueStat ExplosionRadius => _explosionRadius;
        public ValueStat ExplosionDamage => _explosionDamage;

        public ValueStat ExplosionKnockbackPower => _explosionKnockbackPower;

        public void Initialize()
        {
            _health.Initialize();
        }
    }
}
