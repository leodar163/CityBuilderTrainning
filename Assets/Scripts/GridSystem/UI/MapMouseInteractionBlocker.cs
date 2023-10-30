using Cameras;
using GridSystem.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GridSystem.UI
{
    public class MapMouseInteractionBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            GridEventSystem.hoveringActivated = false;
            CameraController.Instance.canZoom = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GridEventSystem.hoveringActivated = true;
            CameraController.Instance.canZoom = true;
        }
    }
}