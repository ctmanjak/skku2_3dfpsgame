using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerMove))]
    [RequireComponent(typeof(PlayerStat))]
    [RequireComponent(typeof(PlayerRotate))]
    public class PlayerEntity : MonoBehaviour
    {
        private PlayerMove _playerMove;
        private PlayerStat _playerStat;

        private void Awake()
        {
            _playerMove = GetComponent<PlayerMove>();
            _playerStat = GetComponent<PlayerStat>();
        }

        private void Start()
        {
            _playerMove.Initialize(_playerStat.MoveSpeed.Value, _playerStat.JumpPower.Value, _playerStat.SprintMultiplier.Value);
            _playerStat.Initialize();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            if (_playerMove.WantsToSprint)
            {
                _playerMove.SetCanSprinting(_playerStat.Stamina.TryConsume(_playerStat.ConsumeStaminaAmountBySprint.Value, deltaTime));
            }
            _playerMove.Move(deltaTime);
            _playerStat.Stamina.Regenerate(deltaTime);
        }
    }
}