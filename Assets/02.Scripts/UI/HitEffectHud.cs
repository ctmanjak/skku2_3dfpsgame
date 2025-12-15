using System.Collections;
using UnityEngine;

namespace UI
{
    public class HitEffectHud : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.5f;
        public IEnumerator Hit()
        {
            gameObject.SetActive(true);
            yield return new WaitForSeconds(_duration);
            gameObject.SetActive(false);
        }
    }
}