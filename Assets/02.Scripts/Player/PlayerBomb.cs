using Core;
using UnityEngine;

namespace Player
{
    public class PlayerBomb : MonoBehaviour
    {
        [SerializeField] private Transform _fireTransform;
        [SerializeField] private Vector3 _fireOffset;
        [SerializeField] private GameObject _bombPrefab;
        [SerializeField] private float _throwPower;

        public void Fire()
        {
            GameObject bomb = PoolManager.Get(_bombPrefab);
            bomb.transform.position = _fireTransform.position + _fireOffset;
            Rigidbody bombBody = bomb.GetComponent<Rigidbody>();
            bombBody.AddForce(Camera.main.transform.forward * _throwPower, ForceMode.Impulse);
        }
    }
}