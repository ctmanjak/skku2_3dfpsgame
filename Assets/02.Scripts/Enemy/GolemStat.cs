using Core;
using UnityEngine;

namespace Enemy
{
    public class GolemStat : EnemyStat
    {
        [SerializeField] private ValueStat _attackDelay;
        [SerializeField] private ValueStat _attackRadius;
        [SerializeField] private ValueStat _knockbackPower;

        public ValueStat AttackDelay => _attackDelay;
        public ValueStat AttackRadius => _attackRadius;
        public ValueStat AttackKnockbackPower => _knockbackPower;
    }
}
