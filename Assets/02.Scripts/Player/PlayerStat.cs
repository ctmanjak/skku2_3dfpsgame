using Core;
using UnityEngine;

namespace Player
{
    public class PlayerStat : MonoBehaviour
    {
        [SerializeField] private ConsumableStat _health;
        [SerializeField] private ConsumableStat _stamina;
        
        [SerializeField] private ValueStat _moveSpeed;
        [SerializeField] private ValueStat _jumpPower;
        [SerializeField] private ValueStat _sprintMultiplier;
        [SerializeField] private ValueStat _consumeStaminaAmountBySprint;
        [SerializeField] private ValueStat _consumeStaminaAmountByDoubleJump;
        [SerializeField] private ValueStat _maxJumpCount;

        public ConsumableStat Health => _health;
        public ConsumableStat Stamina => _stamina;
        
        public ValueStat MoveSpeed => _moveSpeed;
        public ValueStat JumpPower => _jumpPower;
        public ValueStat ConsumeStaminaAmountBySprint => _consumeStaminaAmountBySprint;
        public ValueStat ConsumeStaminaAmountByDoubleJump => _consumeStaminaAmountByDoubleJump;
        public ValueStat SprintMultiplier => _sprintMultiplier;
        public ValueStat MaxJumpCount => _maxJumpCount;

        public void Initialize()
        {
            _health.Initialize();
            _stamina.Initialize();
        }
    }
}
