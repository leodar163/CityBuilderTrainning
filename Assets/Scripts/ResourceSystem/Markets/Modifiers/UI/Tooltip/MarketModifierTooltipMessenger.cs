using Format;
using Localization;
using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ResourceSystem.Markets.Modifiers.UI.Tooltip
{
    public class MarketModifierTooltipMessenger : MonoBehaviour, ITooltipMessenger, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextTooltipMessageUI _tooltipMessage;
        [SerializeField] private MarketModifierUI _marketModifierUI;
        public ITooltipMessenger tooltipMessengerSelf => this;
        public TooltipMessageUI tooltipMessage => _tooltipMessage;
        public void UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
            if (messageUI is TextTooltipMessageUI tooltipMessageUI)
            {
                tooltipMessageUI.SetTexts(_marketModifierUI.Modifier.EffectName, _marketModifierUI.Modifier.GetFormatMessage(false));
            }
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