using Cameras;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GridSystem.UI
{
    public class MapMouseInteractionBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            GridManager.hoveringActivated = false;
            CameraController.Instance.canZoom = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GridManager.hoveringActivated = true;
            CameraController.Instance.canZoom = true;
        }
    }
}