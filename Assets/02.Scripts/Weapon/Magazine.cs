using System;
using Core;
using UnityEngine;

namespace Weapon
{
    [Serializable]
    public class Magazine
    {
        [SerializeField] private ConsumableStat _ammo;
        [SerializeField] private int _consumeAmmoAmount = 1;

        public void Initialize()
        {
            _ammo.Initialize();
        }

        public bool TryLoadAmmo()
        {
            return _ammo.TryDecrease(_consumeAmmoAmount);
        }

        public int GetLeftAmmo()
        {
            return (int)_ammo.Value;
        }
    }
}