using UnityEngine;
using Weapon;

namespace Player
{
    public class PlayerGunFire : MonoBehaviour
    {
        [SerializeField] private Transform _fireTransform;
        [SerializeField] private ParticleSystem _hitEffect;
        private Gun _gun;

        public void Initialize(Gun gun)
        {
            _gun = gun;
        }
        
        public bool TryFire()
        {
            return _gun.TryFire(Camera.main.transform.forward);
        }
    }
}