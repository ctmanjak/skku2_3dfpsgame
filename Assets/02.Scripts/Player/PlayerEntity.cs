using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerMove))]
    [RequireComponent(typeof(PlayerStat))]
    [RequireComponent(typeof(PlayerRotate))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerEntity : MonoBehaviour
    {
        private PlayerMove _playerMove;
        private PlayerStat _playerStat;
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerMove = GetComponent<PlayerMove>();
            _playerStat = GetComponent<PlayerStat>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            _playerMove.Initialize(_playerStat.MoveSpeed.Value, _playerStat.JumpPower.Value, _playerStat.SprintMultiplier.Value);
            _playerStat.Initialize();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            if (_playerMove.IsGrounded())
            {
                _playerInput.Grounding();
                _playerMove.Grounding();
            }
            if (_playerInput.WantsToSprint && _playerStat.Stamina.TryConsume(_playerStat.ConsumeStaminaAmountBySprint.Value, deltaTime))
            {
                _playerMove.SetSprintMultiplier(_playerStat.SprintMultiplier.Value);
            }

            if (_playerInput.WantsToJump && _playerMove.JumpCount < _playerStat.MaxJumpCount.Value)
            {
                if (_playerMove.JumpCount == 0 || _playerStat.Stamina.TryDecrease(_playerStat.ConsumeStaminaAmountByDoubleJump.Value))
                {
                    _playerMove.Jump();
                }
            }
            _playerMove.Move(deltaTime);
            _playerStat.Stamina.Regenerate(deltaTime);
        }
    }
}