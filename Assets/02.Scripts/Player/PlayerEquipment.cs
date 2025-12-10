using System;
using System.Collections;
using UnityEngine;
using Weapon;

namespace Player
{
    public class PlayerEquipment : MonoBehaviour
    {
        [SerializeField] private Gun _gun;
        [SerializeField] private Magazine[] _magazines;

        private int _currentMagazineIndex;

        public Gun EquippedGun => _gun;

        public event Action OnReloadStart;
        public event Action OnReloadEnd;
        public event Action<Gun, Magazine[]> OnMagazineChanged;
        public event Action<Gun> OnFire
        {
            add    => _gun.OnFire += value;
            remove => _gun.OnFire -= value;
        }
        public event Action<float> OnReload
        {
            add    => _gun.OnReload += value;
            remove => _gun.OnReload -= value;
        }

        public void Initialize()
        {
            foreach (var magazine in _magazines)
            {
                magazine.Initialize();
            }

            if (_magazines.Length > 0)
            {
                _currentMagazineIndex = 0;
                _gun.Initialize(_magazines[_currentMagazineIndex]);
            }
            
            OnMagazineChanged?.Invoke(_gun, _magazines);
        }

        public void Reload(Magazine magazine)
        {
            if (_gun.IsReloading) return;
            StartCoroutine(ReloadRoutine(magazine));
        }
        
        private IEnumerator ReloadRoutine(Magazine magazine)
        {
            OnReloadStart?.Invoke();
            yield return StartCoroutine(_gun.Reload(magazine));
            OnReloadEnd?.Invoke();
            OnMagazineChanged?.Invoke(_gun, _magazines);
        }
        
        public bool IsAmmoLeftInGun()
        {
            return _gun.IsAmmoLeft();
        }

        public Magazine GetNextMagazine()
        {
            _currentMagazineIndex++;
            if (_currentMagazineIndex >= _magazines.Length) _currentMagazineIndex = 0;
            return _magazines[_currentMagazineIndex];
        }
    }
}
