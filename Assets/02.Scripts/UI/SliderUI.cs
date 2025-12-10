using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SliderUI : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        public void Activate()
        {
            gameObject.SetActive(true);
        }
        
        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
        
        public void ChangeValue(float value)
        {
            _slider.value = value;
        }
    }
}