using System;
using System.Collections;
using Core;
using UnityEngine;
using Weapon;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerEquipment : MonoBehaviour
    {
        [SerializeField] private CameraRotate _cameraRotate;
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
                _gun.OnFire += gun =>
                    _cameraRotate.AddRecoil(Random.Range(-gun.RecoilPowerX, gun.RecoilPowerX), -gun.RecoilPowerY);
            }
            
            OnMagazineChanged?.Invoke(_gun, _magazines);
        }

        public void Reload()
        {
            if (_gun.IsReloading) return;
            _gun.StartReload();
            StartCoroutine(ReloadRoutine(GetNextMagazine()));
        }
        
        private IEnumerator ReloadRoutine(Magazine magazine)
        {
            OnReloadStart?.Invoke();
            yield return _gun.ReloadRoutine(magazine);
            OnReloadEnd?.Invoke();
            OnMagazineChanged?.Invoke(_gun, _magazines);
        }
        
        public bool IsAmmoLeftInGun()
        {
            return _gun.IsAmmoLeft();
        }

        public Magazine GetNextMagazine()
        {
            if (++_currentMagazineIndex >= _magazines.Length) _currentMagazineIndex = 0;
            return _magazines[_currentMagazineIndex];
        }
    }
}
