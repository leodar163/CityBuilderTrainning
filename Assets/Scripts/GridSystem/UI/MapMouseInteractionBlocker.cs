using System;
using Cameras;
using GridSystem.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GridSystem.UI
{
    public class MapMouseInteractionBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private bool _isHovered;

        public bool isHovered => _isHovered;
        
        private void OnDisable()
        {
            EndHover();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            BeginHover();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            EndHover();
        }

        private void BeginHover()
        {
            if (_isHovered) return;
            _isHovered = !_isHovered;
            
            GridEventSystem.hoveringActivated = false;
            CameraController.Instance.canZoom = false;
        }

        private void EndHover()
        {
            if (!_isHovered) return;
            _isHovered = !_isHovered;
            GridEventSystem.hoveringActivated = true;
            CameraController.Instance.canZoom = true;
        }
    }
}