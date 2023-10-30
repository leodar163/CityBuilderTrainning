using Format;
using ResourceSystem.Markets.Needs;
using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

namespace ResourceSystem.Markets.UI.Tooltip
{
    public class HappinessSliderTooltip : MonoBehaviour, ITooltipMessenger, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private HappinessSlider _happinessSlider;
        [Space]
        [SerializeField] private LocalizedString _localisedHappiness;
        [SerializeField] private LocalizedString _localizedNeedIn;
        [SerializeField] private LocalizedString _localizedEffect;
        [SerializeField] private LocalizedString _localizedEfficiency;
        [Space]
        [SerializeField] private TextTooltipMessageUI _TextTooltip;
        public ITooltipMessenger tooltipMessengerSelf => this;
        public TooltipMessageUI tooltipMessage => _TextTooltip;

        private string FormatHappiness()
        {
            NeedsSet needsSet = _happinessSlider.market.needsSet;
            string format = _localisedHappiness.GetLocalizedString();
            format +=
                $" : <color=#{(needsSet.happiness > 0 ? FormatManager.positiveColorHTML : FormatManager.negativeColorHTML)}>" +
                $"{needsSet.happiness}</color> (<color=#{(needsSet.happinessDelta > 0 ? FormatManager.positiveColorHTML : FormatManager.negativeColorHTML)}>" +
                $"{(needsSet.happinessDelta >= 0 ? "+" :"")}{needsSet.happinessDelta}</color>)";
            return format;
        }

        private string FormatHappinessDeltaDetails()
        {
            NeedsSet needsSet = _happinessSlider.market.needsSet;
            string needIn = _localizedNeedIn.GetLocalizedString();

            string format = "";
            foreach (var resourceNeed in needsSet._happinessDeltaDetails.Keys)
            {
                if ( needsSet._happinessDeltaDetails[resourceNeed] == 0) continue;
                
                bool positiveOutcome = needsSet._happinessDeltaDetails[resourceNeed] > 0;
                
                format += $"{needIn} {resourceNeed.resourceName} <color=#" +
                          $"{(positiveOutcome ? FormatManager.positiveColorHTML : FormatManager.negativeColorHTML)}>" +
                          $"{(positiveOutcome ? "+" : "")}" +
                          $"{needsSet._happinessDeltaDetails[resourceNeed]}</color>\n";
            }

            format += $"\n{FormatManager.separator}\n";
            format += $"{_localizedEffect.GetLocalizedString()}\n" +
                      $"<color=#{(needsSet.prodBonus > 0 ? FormatManager.positiveColorHTML : FormatManager.negativeColorHTML)}>" +
                      $"{FormatManager.Plus(needsSet.prodBonus)}{Mathf.RoundToInt(needsSet.prodBonus * 100)}%</color> " +
                      $"{_localizedEfficiency.GetLocalizedString()}";
            
            return format;
        }
            
        public void UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
            if (messageUI is TextTooltipMessageUI textMessage)
            {
                textMessage.SetTexts(FormatHappiness(), FormatHappinessDeltaDetails());
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