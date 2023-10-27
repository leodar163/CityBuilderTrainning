using System;
using System.Collections.Generic;
using ResourceSystem.Markets.Interactions;
using TimeSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private Slider _unrestSlider;
        [SerializeField] private TextMeshProUGUI _unrestText;
        
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

        protected override void Awake()
        {
            base.Awake();
            _unrestSlider.maxValue = 100;
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
                float unrest = Mathf.RoundToInt(_market._peopleNeedManager.unrest * 10000) / 100f;
                _unrestSlider.value = unrest;
                _unrestText.SetText($"{unrest}/\n100");
                
                _unrestSlider.gameObject.SetActive(true);
                _unrestText.gameObject.SetActive(true);
            }
            else
            {
                _unrestSlider.gameObject.SetActive(false);
                _unrestText.gameObject.SetActive(false);
            }
        }

        public override void OpenPanel()
        {
            base.OpenPanel();
            _market = MarketInteractor.SelectedMarket;
       
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
            
            foreach (var value in _resourceValuesUI)
            {
                value.Market = null;
            }
            
            _panel.gameObject.SetActive(false);
        }
    }
}