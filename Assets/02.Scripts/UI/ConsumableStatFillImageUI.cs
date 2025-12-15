using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ConsumableStatFillImageUI : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void ChangeValue(ConsumableStat stat)
        {
            _image.fillAmount = stat.Value / stat.MaxValue;
        }
    }
}