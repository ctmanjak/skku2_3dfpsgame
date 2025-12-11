using System;
using System.Collections;
using UnityEngine;

namespace Weapon
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] private Transform _fireTransform;
        [SerializeField] private ParticleSystem _hitEffect;

        [SerializeField] private float _reloadDuration = 1.6f;
        [SerializeField] private float _fireRate = 2f;
        [SerializeField] private float _recoilPowerX = 1f;
        [SerializeField] private float _recoilPowerY = 1f;

        private float _reloadTimer;
        private float _fireTimer;
        private Magazine _magazine;
        private bool _isReloading;

        public bool IsReloading => _isReloading;
        public float RecoilPowerX => _recoilPowerX;
        public float RecoilPowerY => _recoilPowerY;
        public Magazine Magazine => _magazine;

        public event Action<Gun> OnFire;
        public event Action<float> OnReload;

        public void Initialize(Magazine magazine)
        {
            _magazine = magazine;
            OnFire?.Invoke(this);
        }

        private void Update()
        {
            _fireTimer -= Time.deltaTime;
        }

        public bool IsAmmoLeft()
        {
            return _magazine.GetLeftAmmo() > 0;
        }
        
        public bool TryFire()
        {
            if (_fireTimer > 0f || _isReloading)
            {
                return false;
            }

            if (!_magazine.TryLoadAmmo()) return false;
            
            Ray ray = new Ray(_fireTransform.position, Camera.main.transform.forward);
            RaycastHit hitInfo = new RaycastHit();
            Fire(ray, hitInfo);

            return true;
        }
        
        private void Fire(Ray ray, RaycastHit hitInfo)
        {
            bool isHit = Physics.Raycast(ray, out hitInfo);
            if (isHit)
            {   
                _hitEffect.transform.position = hitInfo.point;
                _hitEffect.transform.forward = hitInfo.normal;
                _hitEffect.Play();
            }
            
            OnFire?.Invoke(this);

            _fireTimer = 1 / _fireRate;
        }

        public void StartReload()
        {
            _isReloading = true;
            _reloadTimer = 0;
        }

        public IEnumerator ReloadRoutine(Magazine magazine)
        {
            while (_reloadTimer < _reloadDuration)
            {
                yield return null;
                _reloadTimer += Time.deltaTime;

                OnReload?.Invoke(_reloadTimer / _reloadDuration);
            }
            
            _magazine = magazine;
            _isReloading = false;
        }

        public int GetLeftAmmo()
        {
            return _magazine.GetLeftAmmo();
        }
    }
}