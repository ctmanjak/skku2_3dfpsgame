using Core;
using UnityEngine;

namespace Enemy
{
    public class EnemyStat : MonoBehaviour
    {
        [SerializeField] protected ConsumableStat _health;
        [SerializeField] protected ValueStat _moveSpeed;
        [SerializeField] protected ValueStat _attackDistance;
        [SerializeField] protected ValueStat _attackSpeed;
        [SerializeField] protected ValueStat _attackDamage;
        [SerializeField] protected ValueStat _jumpPower;

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
