using UnityEngine;
using Weapon;

namespace UI
{
    public class GunMuzzleFlash : MonoBehaviour
    {
        [SerializeField] private Gun _gun;
        [SerializeField] private Transform _initTransform;
        [SerializeField] private GameObject[] _muzzleFlashPrefabs;
        [SerializeField] private float _aliveTime = 0.1f;
        
        private GameObject _currentMuzzleFlash;

        private void Awake()
        {
            _gun.OnFire += Fire;
        }

        private void LateUpdate()
        {
            if (_currentMuzzleFlash) _currentMuzzleFlash.transform.rotation = Camera.main.transform.rotation;
        }

        private void Fire(Gun _)
        {
            _currentMuzzleFlash = Instantiate(_muzzleFlashPrefabs[Random.Range(0, _muzzleFlashPrefabs.Length)], _initTransform);
            
            Destroy(_currentMuzzleFlash, _aliveTime);
        }
    }
}