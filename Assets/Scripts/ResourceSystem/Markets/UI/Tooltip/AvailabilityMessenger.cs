using Format;
using Localization;
using TMPro;
using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ResourceSystem.Markets.UI.Tooltip
{
    public class AvailabilityMessenger : MonoBehaviour, ITooltipMessenger, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI _availability;
        [SerializeField] private TextTooltipMessageUI _textTooltipMessageUI;

        private ResourceValue _resourceValue;

        public ResourceValue ResourceValue
        {
            get => _resourceValue;
            set
            {
                _resourceValue = value;
                UpdateText();
            }
        }
        
        public ITooltipMessenger tooltipMessengerSelf => this;
        public TooltipMessageUI tooltipMessage => _textTooltipMessageUI;
        public void UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
            if (messageUI is TextTooltipMessageUI textTooltipMessageUI)
            {
                textTooltipMessageUI.SetTexts(FormatTitle(), VariableNameManager.AvailabilityDesc);
            }
        }

        private string FormatTitle()
        {
            string title = VariableNameManager.AvailabilityName;

            if (_resourceValue != null)
            {
                title += $" ({FormatAvailabilityAmount()})";
            }
            
            return title;
        }
        
        private void UpdateText()
        {
            if (_resourceValue == null) _availability.SetText("NaN");
            _availability.SetText(FormatAvailabilityAmount());
        }

        private string FormatAvailabilityAmount()
        {
            return _resourceValue.availability <= 0
                ? $"<color=#{FormatManager.negativeColorHTML}>{VariableNameManager.ShortageName}</color>"
                : Mathf.RoundToInt(_resourceValue.availability * 100) + "%";
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipMessengerSelf.SubToTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipMessengerSelf.UnsubFromTooltip();
        }
    }
}