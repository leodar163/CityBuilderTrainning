﻿using System;
using GridSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Utils;

namespace UI
{
    public abstract class PanelUI<T> : Singleton<T>, IPointerEnterHandler, IPointerExitHandler
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
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            GridManager.hoveringActivated = true;
        }
    }
}