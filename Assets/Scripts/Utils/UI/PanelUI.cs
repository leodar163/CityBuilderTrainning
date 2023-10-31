using System;
using UnityEngine;
using UnityEngine.Events;

namespace Utils.UI
{
    public abstract class PanelUI<T> : Singleton<T>, IPanel
        where T : PanelUI<T>
    {
        public bool isOpen { get; private set; } = true;
        [SerializeField] private bool _closeOnStart = true;

        public UnityEvent onOpen;
        public UnityEvent onClose;
        
        protected virtual void Start()
        {
            if (_closeOnStart)
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
            IPanel.PutFocusOnPanel(this);
            onOpen.Invoke();
        }

        public virtual void ClosePanel()
        {
            if (!isOpen) return;
            isOpen = false;
            IPanel.NotifyClosingPanel(this);
            onClose.Invoke();
        }
    }
}