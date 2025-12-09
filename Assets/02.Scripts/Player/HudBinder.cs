using System;
using UI;
using UnityEngine;

namespace Player
{
    public class HudBinder : MonoBehaviour
    {
        [SerializeField] private SliderUI _healthSliderUI;
        [SerializeField] private SliderUI _staminaSliderUI;
        [SerializeField] private PlayerStat _playerStat;
        
        private void OnEnable()
        {
            _playerStat.Health.OnValueChanged += _healthSliderUI.ChangeValue;
            _playerStat.Stamina.OnValueChanged += _staminaSliderUI.ChangeValue;
        }

        private void OnDisable()
        {
            _playerStat.Health.OnValueChanged -= _healthSliderUI.ChangeValue;
            _playerStat.Stamina.OnValueChanged -= _staminaSliderUI.ChangeValue;
        }
    }
}