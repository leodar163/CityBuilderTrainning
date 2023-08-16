using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

namespace ToolTipSystem
{
    public class ToolTipStringMessenger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IToolTipSpeaker
    {
        public LocalizedString message;

        public bool isHovered { get; private set; }
        
        private void Update()
        {
            if (isHovered)
            {
                ToolTip.Sub(this);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
        }

        public ToolTipMessage ToToolTipMessage()
        {
            return new ToolTipMessage
            {
                title = message.GetLocalizedString()
            };
        }
    }
}