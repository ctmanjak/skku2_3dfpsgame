using System;
using System.Collections;
using Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ConsumableStatFillImageUI : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        
        [Header("Juicy")]
        [SerializeField] private bool _juicy;
        [SerializeField] private Image _delayImage;
        [SerializeField] private float _delayWaitTime = 0.5f;
        [SerializeField] private float _delayDuration = 1f;
        [SerializeField] private float _shakeDuration = 0.3f;
        [SerializeField] private float _shakeIntensity = 3f;

        private float _delayTimer;
        private Coroutine _juicyCoroutine;
        private Vector3 _originPosition;

        private float _previousFillAmount;

        private void Start()
        {
            _originPosition = transform.localPosition;
        }

        public void ChangeValue(ConsumableStat stat)
        {
            float fillAmount = stat.Value / stat.MaxValue;
            _fillImage.fillAmount = fillAmount;

            if (_previousFillAmount > fillAmount && _juicy)
            {
                if (_juicyCoroutine != null) StopCoroutine(_juicyCoroutine);
                _juicyCoroutine = StartCoroutine(ChangeDelayImage(fillAmount));
                transform.DOShakePosition(_shakeDuration, _shakeIntensity)
                    .OnComplete(() => transform.localPosition = _originPosition);
            }

            _previousFillAmount = fillAmount;
        }

        private IEnumerator ChangeDelayImage(float amount)
        {
            _delayImage.gameObject.SetActive(true);
            
            _delayImage.fillAmount = Mathf.Max(_delayImage.fillAmount, _previousFillAmount);
            _delayTimer = _delayDuration;
            yield return new WaitForSeconds(_delayWaitTime);
            
            while (_delayTimer > 0f)
            {
                yield return null;
                _delayImage.fillAmount = Mathf.Lerp(_delayImage.fillAmount, amount, (_delayDuration - _delayTimer) / _delayDuration);
                _delayImage.fillAmount = Mathf.Max(_delayImage.fillAmount, _fillImage.fillAmount);
                _delayTimer -= Time.deltaTime;
            }
            
            _delayImage.gameObject.SetActive(false);
        }
    }
}