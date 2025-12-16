using Core;
using UnityEngine;

namespace Enemy
{
    public class EnemyStat : MonoBehaviour
    {
        [SerializeField] private ConsumableStat _health;
        [SerializeField] private ValueStat _moveSpeed;
        [SerializeField] private ValueStat _attackDistance;
        [SerializeField] private ValueStat _attackSpeed;
        [SerializeField] private ValueStat _attackDamage;
        [SerializeField] private ValueStat _jumpPower;

        public ConsumableStat Health => _health;
        
        public ValueStat MoveSpeed => _moveSpeed;
        public ValueStat AttackDistance => _attackDistance;
        public ValueStat AttackSpeed => _attackSpeed;
        public ValueStat AttackDamage => _attackDamage;
        public ValueStat JumpPower => _jumpPower;

        public void Initialize()
        {
            _health.Initialize();
        }
    }
}
