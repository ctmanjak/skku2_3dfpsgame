using UnityEngine;

namespace Core
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        private void Update()
        {
            transform.position = _target.position;
        }
    }
}
