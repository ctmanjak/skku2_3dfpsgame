using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class ConsumableStat
    {
        [SerializeField] private float _maxValue;
        [SerializeField] private float _value;

        [SerializeField] private float _regenerateAmount;
        
        public float Value => _value;
        public float MaxValue => _maxValue;

        public event Action<ConsumableStat> OnValueChanged;

        public void Initialize()
        {
            SetValue(_maxValue);
        }

        public void IncreaseMax(float amount)
        {
            SetMaxValue(_maxValue + amount);
        }

        public void DecreaseMax(float amount)
        {
            SetMaxValue(_maxValue - amount);
        }

        public void SetMaxValue(float amount)
        {
            _maxValue = amount;
            OnValueChanged?.Invoke(this);
        }

        public void Regenerate(float deltaTime)
        {
            SetValue(_value + _regenerateAmount * deltaTime);
        }

        public bool TryConsume(float amount, float deltaTime)
        {
            return TryDecrease(amount * deltaTime);
        }

        public bool TryDecrease(float amount)
        {
            if (_value < amount) return false;
            
            Decrease(amount);
            return true;
        }

        public void Decrease(float amount)
        {
            SetValue(_value - amount);
        }

        public void SetValue(float amount)
        {
            _value = Mathf.Clamp(amount, 0f, _maxValue);
            OnValueChanged?.Invoke(this);
        }
    }
}
