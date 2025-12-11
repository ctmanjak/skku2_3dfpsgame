using UnityEngine;

namespace Enemy
{
    public class EnemyRotate : MonoBehaviour
    {
        public void Rotate(float x, float y)
        {
            transform.rotation = Quaternion.Euler(x, y, 0f);
        }
    }
}
