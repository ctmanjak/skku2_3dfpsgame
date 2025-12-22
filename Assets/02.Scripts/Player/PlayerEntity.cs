using System;
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
        private static readonly int _speedHash = Animator.StringToHash("Speed");
        private static readonly int _attackHash = Animator.StringToHash("Attack");
        private static readonly int _jumpHash = Animator.StringToHash("Jump");
        private static readonly int _death = Animator.StringToHash("Death");
        private static readonly int _throw = Animator.StringToHash("Throw");

        [SerializeField] private CameraFollow _cameraFollow;
        [SerializeField] private CameraRotate _cameraRotate;

        [SerializeField] private GameObject _normalCrosshair;
        [SerializeField] private GameObject _zoomInCrosshair;
        
        private PlayerMove _playerMove;
        private PlayerStat _playerStat;
        private PlayerInput _playerInput;
        private PlayerBomb _playerBomb;
        private PlayerEquipment _playerEquipment;
        private PlayerGunFire _playerGunFire;
        private PlayerRotate _playerRotate;
        private Animator _animator;

        private bool _isDead;
        private EZoomMode _zoomMode = EZoomMode.Normal;

        public event Action OnHit;

        private void Awake()
        {
            _playerMove = GetComponent<PlayerMove>();
            _playerStat = GetComponent<PlayerStat>();
            _playerInput = GetComponent<PlayerInput>();
            _playerBomb = GetComponent<PlayerBomb>();
            _playerEquipment = GetComponent<PlayerEquipment>();
            _playerGunFire = GetComponent<PlayerGunFire>();
            _playerRotate = GetComponent<PlayerRotate>();
            
            _animator = GetComponentInChildren<Animator>();
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
            if (_isDead) return;
            float deltaTime = Time.deltaTime;

            if (_playerInput.ZoomInPressed)
            {
                if (_zoomMode == EZoomMode.Normal)
                {
                    _normalCrosshair.SetActive(false);
                    _zoomInCrosshair.SetActive(true);
                    _zoomMode = EZoomMode.ZoomIn;
                }
                else
                {
                    _normalCrosshair.SetActive(true);
                    _zoomInCrosshair.SetActive(false);
                    _zoomMode = EZoomMode.Normal;
                }
                _playerInput.ConsumeZoomIn();
            }

            if (_playerInput.UnlockCursorPressed)
            {
                CursorManager.Instance.UnlockCursor();
                _playerInput.ConsumeUnlockCursor();
            }

            if (_playerMove.IsGrounded()) _playerMove.Grounding();
            if (_playerInput.SprintHeld && _playerStat.Stamina.TryConsume(_playerStat.ConsumeStaminaAmountBySprint.Value, deltaTime))
            {
                _playerMove.SetSprintMultiplier(_playerStat.SprintMultiplier.Value);
            }

            if (_playerInput.JumpPressed && _playerMove.JumpCount < _playerStat.MaxJumpCount.Value)
            {
                if (_playerMove.JumpCount == 0 || _playerStat.Stamina.TryDecrease(_playerStat.ConsumeStaminaAmountByDoubleJump.Value))
                {
                    _animator.SetTrigger(_jumpHash);
                    _playerMove.Jump();
                    _playerInput.ConsumeJump();
                }
            }

            if (_playerInput.BombPressed && _playerStat.BombCount.TryDecrease(1f))
            {
                _animator.SetTrigger(_throw);
                _playerInput.ConsumeBomb();
            }

            if (_playerInput.FireHeld)
            {
                if (!_playerEquipment.IsAmmoLeftInGun())
                {
                    _playerEquipment.Reload();
                }
                else
                {
                    if (_playerGunFire.TryFire())
                    {
                        _animator.SetTrigger(_attackHash);
                    }
                }
            }

            if (_playerInput.ReloadPressed)
            {
                _playerEquipment.Reload();
                _playerInput.ConsumeReload();
            }


            ViewTypeConfig viewTypeConfig = _cameraFollow.GetViewTypeConfig();
            if (_playerInput.ChangeViewPressed)
            {
                _cameraFollow.NextViewType();
                viewTypeConfig = _cameraFollow.GetViewTypeConfig();
                if (viewTypeConfig.IsRotationLock)
                {
                    _cameraRotate.LockRotation(viewTypeConfig.Rotation, _cameraFollow.TransformDuration);
                }
                else
                {
                    _cameraRotate.UnlockRotation();
                }
                
                _playerInput.ConsumeChangeView();
                
            }

            if (viewTypeConfig.IsRotationLock)
            {
                _playerRotate.LockAim(new Vector2(_cameraRotate.BaseX, _cameraRotate.BaseY));
            }
            else
            {
                _playerRotate.Aim(_playerInput.AimAxis);
            }
            
            _animator.SetFloat(_speedHash, _playerInput.MoveAxis.magnitude);
            _playerMove.Move(_playerInput.MoveAxis, deltaTime);
            _playerStat.Stamina.Regenerate(deltaTime);
            _playerStat.Health.Regenerate(deltaTime);
        }

        public void TakeDamage(AttackContext context)
        {
            if (_isDead) return;
            
            _playerStat.Health.Decrease(context.Damage);
            
            OnHit?.Invoke();

            if (_playerStat.Health.Value <= 0f)
            {
                _isDead = true;
                _animator.SetTrigger(_death);
            }
        }

        public void Knockback(Vector3 direction)
        {
        }
    }
}