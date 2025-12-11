using UnityEngine;

namespace Enemy
{
    public class EnemyRotate : MonoBehaviour
    {
        public void Rotate(Vector3 direction)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
