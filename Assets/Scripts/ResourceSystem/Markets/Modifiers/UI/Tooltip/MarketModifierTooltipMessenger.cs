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
                tooltipMessageUI.SetTexts(_marketModifierUI.Modifier.EffectName, FormatModifier());
            }
        }

        private string FormatModifier()
        {
            string format = "";
            foreach (var order in _marketModifierUI.Modifier.Orders)
            {
                if (order.quantity <= 0) continue;
                format += $"+{order.quantity} " +
                          $"{(order.orderType == OrderType.Demand ? $"<color=#{FormatManager.negativeColorHTML}>{VariableNameManager.DemandName}" : $"<color=#{FormatManager.positiveColorHTML}>{VariableNameManager.OfferName}")} " +
                          $"</color>{order.resource.resourceName}";
            }

            if (format.Length > 0 && _marketModifierUI.Modifier.Multipliers.Count > 0)
                format += '\n' + FormatManager.separator + '\n';

            foreach (var mult in _marketModifierUI.Modifier.Multipliers)
            {
                if (mult.quantity == 0) continue;
                string posNeg = mult.quantity > 0
                    ? $"<color=#{FormatManager.positiveColorHTML}>+"
                    : $"<color=#{FormatManager.negativeColor}>";
                format +=
                    $"{posNeg}{Mathf.RoundToInt(mult.quantity * 1000) / 10f}%</color> " +
                    $"{VariableNameManager.ProductionName} ({mult.resource.resourceName})";
            }

            return format;
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