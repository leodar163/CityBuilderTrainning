using Cameras;
using GridSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils.UI
{
    public abstract class PanelUI<T> : Singleton<T>, IPointerEnterHandler, IPointerExitHandler, IPanel
        where T : PanelUI<T>
    {
        public bool isOpen { get; private set; }
        [SerializeField] private bool _closeOnAwake;

        protected virtual void Awake()
        {
            if (_closeOnAwake)
                ClosePanel();   
        }
        
        public void SwitchPanelOpening()
        {
            if (isOpen)
            {
                ClosePanel();
            }
            else
            {
                OpenPanel();
            }
        }
        
        public virtual void OpenPanel()
        {
            if (isOpen) return;
            isOpen = true;
        }

        public virtual void ClosePanel()
        {
            if (!isOpen) return;
            isOpen = false;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            GridManager.hoveringActivated = false;
            CameraController.Instance.canZoom = false;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            GridManager.hoveringActivated = true;
            CameraController.Instance.canZoom = true;
        }
    }
}