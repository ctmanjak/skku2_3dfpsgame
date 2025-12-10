using UnityEngine;

namespace Player
{
    public class PlayerBomb : MonoBehaviour
    {
        [SerializeField] private Transform _fireTransform;
        [SerializeField] private Bomb _bombPrefab;
        [SerializeField] private float _throwPower;

        public void Fire()
        {
            Bomb bomb = Instantiate(_bombPrefab, _fireTransform.position, Quaternion.identity);
            Rigidbody bombBody = bomb.GetComponent<Rigidbody>();
            bombBody.AddForce(Camera.main.transform.forward * _throwPower, ForceMode.Impulse);
        }
    }
}