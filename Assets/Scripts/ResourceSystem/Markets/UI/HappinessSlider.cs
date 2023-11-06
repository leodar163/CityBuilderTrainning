using System.Globalization;
using Format;
using ResourceSystem.Markets.Needs;
using TMPro;
using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Utils;

namespace ResourceSystem.Markets.UI
{
    public class HappinessSlider : Singleton<HappinessSlider>
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _text;

        private Market _market;

        public Market market
        {
            get => _market;
            set
            {
                _market = value;
                UpdateSlider();
            }
        }
        
        public void UpdateSlider()
        {
            if (_market == null || _market.isEcosystem)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            _text.SetText(FormatHappinessText());
            NeedsSet needSet = _market.needsSet;
            if (_slider != null)
            {
                _slider.minValue = needSet.minHappiness;
                _slider.maxValue = needSet.maxHappiness;
                _slider.value = needSet.happiness;
            }
        }

        private string FormatHappinessText()
        {
            string happiness = _market.needsSet.happiness <= 0 ? $"<color=#{FormatManager.negativeColorHTML}>" : "";
            happiness += Mathf.RoundToInt(_market.needsSet.happiness).ToString(CultureInfo.CurrentCulture) + "%";
            return happiness;
        }
    }
}