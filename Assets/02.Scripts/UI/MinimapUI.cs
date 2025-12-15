using Core;
using UnityEngine;

namespace UI
{
    public class MinimapUI : MonoBehaviour
    {
        [SerializeField] private MinimapCamera _minimapCamera;
        [SerializeField] private float _zoomAmount = 10f;

        public void ZoomOut()
        {
            _minimapCamera.ZoomOut(_zoomAmount);
        }
        
        public void ZoomIn()
        {
            _minimapCamera.ZoomIn(_zoomAmount);
        }
    }
}