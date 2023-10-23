using ToolTipSystem.Messages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

namespace ToolTipSystem.Messengers
{
    public class StringTooltipMessenger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ITooltipMessenger
    {
        public LocalizedString LocalizedTitle;
        public LocalizedString localizedMessage;

        [SerializeField] private TextTooltipMessageUI textTooltipMessageTemplate;
        private TextTooltipMessageUI _currentTextMessage;

        public ITooltipMessenger tooltipMessengerSelf => this;
        public TooltipMessageUI tooltipMessage => textTooltipMessageTemplate;

        public void UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
            if (_currentTextMessage == null || _currentTextMessage != messageUI)
            {
                _currentTextMessage = messageUI as TextTooltipMessageUI;
                if (_currentTextMessage == null) return;
            }

            string messageText = localizedMessage.IsEmpty ? "" : localizedMessage.GetLocalizedString();
            _currentTextMessage.SetTexts(LocalizedTitle.GetLocalizedString(), messageText);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipMessengerSelf.SubToTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipMessengerSelf.UnsubFromTooltip();
            _currentTextMessage = null;
        }

       
    }
}