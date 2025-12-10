using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ComsumableStatSliderUI : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        public void ChangeValue(ConsumableStat stat)
        {
            _slider.value = stat.Value / stat.MaxValue;
        }
    }
}