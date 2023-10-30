using ResourceSystem.Markets.Interactions;
using TimeSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.UI;

namespace ResourceSystem.Markets.UI
{
    public class MarketInfoPanel : PanelUI<MarketInfoPanel>
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _marketName;
        [SerializeField] private Image _marketColor;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private HappinessSlider _happinessSlider;

        [Header("Values")]
        [SerializeField] private ResourceValueUI[] _resourceValuesUI;

        private Market _market;

        private void OnEnable()
        {
            TimeManager.onMonthBegins += UpdateDisplay;
        }

        private void OnDisable()
        {
            TimeManager.onMonthBegins -= UpdateDisplay;
        }

        private void UpdateDisplay()
        {
            if (!isOpen) return;
            
            _marketName.SetText(_market.name);
            _marketColor.color = _market.color;

            foreach (var value in _resourceValuesUI)
            {
                value.UpdateDisplay();
            }

            if (!_market.isEcosystem)
            {
                _happinessSlider.UpdateSlider();

                _happinessSlider.gameObject.SetActive(true);
            }
            else
            {
                _happinessSlider.gameObject.SetActive(false);
            }
        }

        public override void OpenPanel()
        {
            base.OpenPanel();
            _market = MarketInteractor.SelectedMarket;
            _happinessSlider.market = _market;
       
            foreach (var value in _resourceValuesUI)
            {
                value.Market = _market;
            }
            
            UpdateDisplay();
            _panel.gameObject.SetActive(true);
        }

        public override void ClosePanel()
        {
            base.ClosePanel();
            _market = null;
            _happinessSlider.market = null;
            
            foreach (var value in _resourceValuesUI)
            {
                value.Market = null;
            }
            
            _panel.gameObject.SetActive(false);
        }
    }
}