using UnityEngine;

namespace Weapon
{
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private GameObject _explosionEffectPrefab;
    
        private void OnCollisionEnter(Collision collision)
        {
            Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}