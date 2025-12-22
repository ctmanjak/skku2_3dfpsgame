using UnityEngine;

namespace Gold
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class GoldExtraGravity : MonoBehaviour
    {
        [Header("Gravity")]
        [Tooltip("1 = 기본 중력 그대로, 2 = 중력을 2배로(추가 중력 1배 더)")]
        [SerializeField] private float _gravityMultiplier = 2.0f;

        [Header("Optional Safety")]
        [Tooltip("낙하 속도 상한(너무 빨라지는 것 방지). 0이면 제한 없음")]
        [SerializeField] private float _maxDownSpeed = 0f;

        [Tooltip("풀링 재사용 시, 활성화될 때 속도를 리셋할지")]
        [SerializeField] private bool _resetVelocityOnEnable = true;

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            if (_resetVelocityOnEnable && _rb != null)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
        }

        private void FixedUpdate()
        {
            if (_rb == null) return;

            // 기본 중력은 Unity가 이미 적용(useGravity=true)하므로,
            // 추가로 (multiplier - 1) 만큼만 더해줌.
            float extra = _gravityMultiplier - 1f;
            if (extra > 0f)
            {
                _rb.AddForce(Physics.gravity * extra, ForceMode.Acceleration);
            }

            if (_maxDownSpeed > 0f)
            {
                Vector3 v = _rb.linearVelocity;
                if (v.y < -_maxDownSpeed)
                {
                    v.y = -_maxDownSpeed;
                    _rb.linearVelocity = v;
                }
            }
        }

        public void SetGravityMultiplier(float multiplier)
        {
            _gravityMultiplier = Mathf.Max(0f, multiplier);
        }
    }
}