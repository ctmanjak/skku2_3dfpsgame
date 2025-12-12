using Core;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerMove))]
    [RequireComponent(typeof(PlayerStat))]
    [RequireComponent(typeof(PlayerRotate))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerBomb))]
    [RequireComponent(typeof(PlayerEquipment))]
    [RequireComponent(typeof(PlayerGunFire))]
    public class PlayerEntity : MonoBehaviour, IDamageable
    {
        [SerializeField] private CameraFollow _cameraFollow;
        
        private PlayerMove _playerMove;
        private PlayerStat _playerStat;
        private PlayerInput _playerInput;
        private PlayerBomb _playerBomb;
        private PlayerEquipment _playerEquipment;
        private PlayerGunFire _playerGunFire;

        private void Awake()
        {
            _playerMove = GetComponent<PlayerMove>();
            _playerStat = GetComponent<PlayerStat>();
            _playerInput = GetComponent<PlayerInput>();
            _playerBomb = GetComponent<PlayerBomb>();
            _playerEquipment = GetComponent<PlayerEquipment>();
            _playerGunFire = GetComponent<PlayerGunFire>();
        }

        private void Start()
        {
            _playerMove.Initialize(_playerStat.MoveSpeed.Value, _playerStat.JumpPower.Value, _playerStat.SprintMultiplier.Value);
            _playerStat.Initialize();
            _playerEquipment.Initialize();
            _playerGunFire.Initialize(_playerEquipment.EquippedGun);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            if (_playerMove.IsGrounded()) _playerMove.Grounding();
            if (_playerInput.SprintHeld && _playerStat.Stamina.TryConsume(_playerStat.ConsumeStaminaAmountBySprint.Value, deltaTime))
            {
                _playerMove.SetSprintMultiplier(_playerStat.SprintMultiplier.Value);
            }

            if (_playerInput.JumpPressed && _playerMove.JumpCount < _playerStat.MaxJumpCount.Value)
            {
                if (_playerMove.JumpCount == 0 || _playerStat.Stamina.TryDecrease(_playerStat.ConsumeStaminaAmountByDoubleJump.Value))
                {
                    _playerMove.Jump();
                    _playerInput.ConsumeJump();
                }
            }

            if (_playerInput.BombPressed && _playerStat.BombCount.TryDecrease(1f))
            {
                _playerBomb.Fire();
                _playerInput.ConsumeBomb();
            }

            if (_playerInput.FireHeld)
            {
                if (!_playerEquipment.IsAmmoLeftInGun())
                {
                    _playerEquipment.Reload();
                } else _playerGunFire.Fire();
            }

            if (_playerInput.ReloadPressed)
            {
                _playerEquipment.Reload();
                _playerInput.ConsumeReload();
            }

            if (_playerInput.ChangeViewPressed)
            {
                _cameraFollow.NextViewType();
                _playerInput.ConsumeChangeView();
            }
            
            _playerMove.Move(_playerInput.MoveAxis, deltaTime);
            _playerStat.Stamina.Regenerate(deltaTime);
        }

        public void TakeDamage(float damage)
        {
            _playerStat.Health.TryDecrease(damage);
        }

        public void Knockback(Vector3 direction)
        {
        }
    }
}