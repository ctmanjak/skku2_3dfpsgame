using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ConsumableStatTextUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _bombCountText;

        public void ChangeValue(ConsumableStat stat)
        {
            _bombCountText.text = $"{stat.Value} / {stat.MaxValue}";
        }
    }
}