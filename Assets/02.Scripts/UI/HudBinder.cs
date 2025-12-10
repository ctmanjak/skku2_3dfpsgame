using Player;
using UnityEngine;

namespace UI
{
    public class HudBinder : MonoBehaviour
    {
        [SerializeField] private PlayerStat _playerStat;
        
        [SerializeField] private ComsumableStatSliderUI _healthSliderUI;
        [SerializeField] private ComsumableStatSliderUI _staminaSliderUI;
        
        [SerializeField] private ConsumableStatTextUI _bombCountTextUI;
        
        private void OnEnable()
        {
            _playerStat.Health.OnValueChanged += _healthSliderUI.ChangeValue;
            _playerStat.Stamina.OnValueChanged += _staminaSliderUI.ChangeValue;
            _playerStat.BombCount.OnValueChanged += _bombCountTextUI.ChangeValue;
        }

        private void OnDisable()
        {
            _playerStat.Health.OnValueChanged -= _healthSliderUI.ChangeValue;
            _playerStat.Stamina.OnValueChanged -= _staminaSliderUI.ChangeValue;
            _playerStat.BombCount.OnValueChanged -= _bombCountTextUI.ChangeValue;
        }
    }
}