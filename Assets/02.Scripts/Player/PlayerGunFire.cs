using UnityEngine;

namespace Player
{
    public class PlayerGunFire : MonoBehaviour
    {
        [SerializeField] private Transform _fireTransform;
        [SerializeField] private ParticleSystem _hitEffect;
        
        private void Update()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                TryFire();
            }
        }
        private void TryFire()
        {
            Ray ray = new Ray(_fireTransform.position, Camera.main.transform.forward);
            RaycastHit hitInfo = new RaycastHit();
            Fire(ray, hitInfo);
        }
        private void Fire(Ray ray, RaycastHit hitInfo)
        {
            bool isHit = Physics.Raycast(ray, out hitInfo);
            if (isHit == true)
            {   
                Debug.Log($"Hit : {hitInfo.transform.name}");
                _hitEffect.transform.position = hitInfo.point;
                _hitEffect.transform.forward = hitInfo.normal;
                // ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                _hitEffect.Play();
            }
        }
    }
}