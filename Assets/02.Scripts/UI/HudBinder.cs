using Player;
using UnityEngine;

namespace UI
{
    public class HudBinder : MonoBehaviour
    {
        [SerializeField] private PlayerEntity _playerEntity;
        [SerializeField] private PlayerStat _playerStat;
        [SerializeField] private PlayerEquipment _playerEquipment;
        
        // [SerializeField] private ConsumableStatSliderUI _healthSliderUI;
        [SerializeField] private ConsumableStatFillImageUI _healthSliderUI;
        [SerializeField] private ConsumableStatSliderUI _staminaSliderUI;
        [SerializeField] private SliderUI _reloadSliderUI;
        
        [SerializeField] private ConsumableStatTextUI _bombCountTextUI;
        
        [SerializeField] private MagazineTextUI _magazineTextUI;

        [SerializeField] private HitEffectHud _hitEffect;
        
        private void OnEnable()
        {
            _playerEntity.OnHit += () => StartCoroutine(_hitEffect.Hit());
            
            _playerStat.Health.OnValueChanged += _healthSliderUI.ChangeValue;
            _playerStat.Stamina.OnValueChanged += _staminaSliderUI.ChangeValue;
            _playerStat.BombCount.OnValueChanged += _bombCountTextUI.ChangeValue;
            
            _playerEquipment.OnFire += _magazineTextUI.ChangeValueOnGun;
            _playerEquipment.OnReload += _reloadSliderUI.ChangeValue;
            _playerEquipment.OnReloadStart += _reloadSliderUI.Activate;
            _playerEquipment.OnReloadEnd += _reloadSliderUI.Deactivate;
            _playerEquipment.OnMagazineChanged += _magazineTextUI.ChangeValueOnMagazines;
        }

        private void OnDisable()
        {
            _playerStat.Health.OnValueChanged -= _healthSliderUI.ChangeValue;
            _playerStat.Stamina.OnValueChanged -= _staminaSliderUI.ChangeValue;
            _playerStat.BombCount.OnValueChanged -= _bombCountTextUI.ChangeValue;
            
            _playerEquipment.OnFire -= _magazineTextUI.ChangeValueOnGun;
            _playerEquipment.OnReload -= _reloadSliderUI.ChangeValue;
            _playerEquipment.OnReloadStart -= _reloadSliderUI.Activate;
            _playerEquipment.OnReloadEnd -= _reloadSliderUI.Deactivate;
            _playerEquipment.OnMagazineChanged -= _magazineTextUI.ChangeValueOnMagazines;
        }
    }
}