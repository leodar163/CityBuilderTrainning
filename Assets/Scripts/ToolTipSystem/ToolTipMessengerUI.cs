using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ToolTipSystem
{
    public class ToolTipMessengerUI : ToolTipMessengerBase, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform _rectTransform;
        private bool _mouseIsIn;

        private void Update()
        {
            if (ToolTipSpeaker != null && _mouseIsIn)
            {
                ToolTip.Sub(ToolTipSpeaker);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseIsIn = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseIsIn = false;
        }
    }
}