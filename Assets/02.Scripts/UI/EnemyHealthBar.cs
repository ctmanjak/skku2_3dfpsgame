using Enemy;
using UnityEngine;

namespace UI
{
    public class EnemyHealthBar : MonoBehaviour
    {
        private EnemyStat _enemyStat;
        [SerializeField] private ConsumableStatFillImageUI _fillImage;

        private void Awake()
        {
            _enemyStat = GetComponent<EnemyStat>();

            _enemyStat.Health.OnValueChanged += _fillImage.ChangeValue;
        }

        private void LateUpdate()
        {
            _fillImage.transform.rotation = Camera.main.transform.rotation;
        }
    }
}