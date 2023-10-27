﻿using System.Collections.Generic;
using Cameras;
using GridSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Utils.UI
{
    public abstract class PanelUI<T> : Singleton<T>, IPanel
        where T : PanelUI<T>
    {
        public bool isOpen { get; private set; }
        [SerializeField] private bool _closeOnAwake;

        public UnityEvent onOpen;
        public UnityEvent onClose;

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

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
           
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            
        }
    }
}