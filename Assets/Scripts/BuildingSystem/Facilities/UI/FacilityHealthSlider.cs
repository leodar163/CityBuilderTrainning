using System.Collections.Generic;
using Format;
using Localization;
using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BuildingSystem.Facilities.UI
{
    public class FacilityHealthSlider : MonoBehaviour, ITooltipMessenger, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private FacilityInfoUI _facilityInfoUI;
        [SerializeField] private TextTooltipMessageUI _tooltipMessage;
        
        public ITooltipMessenger tooltipMessengerSelf => this;
        public TooltipMessageUI tooltipMessage => _tooltipMessage;
        public void UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
            if (messageUI is TextTooltipMessageUI tooltipMessageUI)
            {
                tooltipMessageUI.SetTexts(FormatTitle(), FormatHealthDeltas());
            }
        }

        private string FormatTitle()
        {
            FacilityType facility = _facilityInfoUI.Facility;
            return facility == null
                ? "no_facility"
                : $"{VariableNameManager.HealthName}{FormatManager.DoublePoint}{facility.health} ({FormatManager.FormatDelta(facility.Growth, 1)})";
        }

        private string FormatHealthDeltas()
        {
            FacilityType facility = _facilityInfoUI.Facility;
            if (facility == null) return "";

            Dictionary<string, (float, int)> collapseEffectors = new Dictionary<string, (float, int)>();

            foreach (var effector in facility.GrowthEffectors)
            {
                if (!collapseEffectors.TryAdd(effector.GrowthEffectorName, (effector.Growth, 1)))
                {
                    (float, int) value = collapseEffectors[effector.GrowthEffectorName];
                    value.Item1 += effector.Growth;
                    value.Item2++;
                    collapseEffectors[effector.GrowthEffectorName] = value;
                }
            }

            string format = "";
            
            foreach (var effector in collapseEffectors)
            {
                format +=
                    $"{effector.Key}{FormatManager.DoublePoint}{(effector.Value.Item2 > 1 ? $"({effector.Value.Item2})":"")}" +
                    $"{FormatManager.FormatDelta(effector.Value.Item1)}\n";
            }

            return format;
        }
        
        private void Update()
        {
            FacilityType facility = _facilityInfoUI.Facility;
            if (facility == null) return;
            
            _slider.maxValue = facility.MaxHealth;
            _slider.value = Mathf.RoundToInt(facility.health * 10) / 10f;
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