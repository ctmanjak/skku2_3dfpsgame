using UnityEngine;

namespace Core
{
    public class CameraRotate : MonoBehaviour
    {
        private const float MIN_X = -90f;
        private const float MAX_X = 90f;

        public void Rotate(float x, float y)
        {
            transform.eulerAngles = new Vector3(Mathf.Clamp(x, MIN_X, MAX_X), y);
        }
    }
}
