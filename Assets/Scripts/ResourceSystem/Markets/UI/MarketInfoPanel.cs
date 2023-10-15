using System.Collections.Generic;
using System.Threading.Tasks;
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
        
        [Header("Values")]
        [SerializeField] private ResourceValueUI _resourceValueUITemplate;
        [SerializeField] private RectTransform _resourceValueContainer;
        private readonly List<ResourceValueUI> _resourceValuesUI = new();

        private Market _market;

        private void OnEnable()
        {
            TimeManager.onMonthBegins += UpdateDisplay;
        }

        private void OnDisable()
        {
            TimeManager.onMonthBegins -= UpdateDisplay;
        }

        private void AddResourceValueUI(ResourceValue resourceValue)
        {
            if (Instantiate(_resourceValueUITemplate, _resourceValueContainer)
                .TryGetComponent(out ResourceValueUI resourceValueUI))
            {
                _resourceValuesUI.Add(resourceValueUI);
                resourceValueUI.ResourceValue = resourceValue;
            }
        }

        private void RemoveResourceValueUI(ResourceValueUI resourceValueUI)
        {
            if (!_resourceValuesUI.Contains(resourceValueUI)) return;

            _resourceValuesUI.Remove(resourceValueUI);
            Destroy(resourceValueUI.gameObject);
        }

        public void UpdateDisplay()
        {
            if (!isOpen) return;
            
            _marketName.SetText(_market.name);
            _marketColor.color = _market.color;
            
            //Remove extra resource value ui that doesn't have valid resource value.
            foreach (var value in _resourceValuesUI.ToArray())
            {
                if (value.ResourceValue == null
                    || !_market._resourceValues.Contains(value.ResourceValue))
                {
                    RemoveResourceValueUI(value);
                }
            }
            
            foreach (var resourceValue in _market._resourceValues)
            {
                if (!TryGetResourceValue(resourceValue, out ResourceValueUI resourceValueUI))
                {
                    AddResourceValueUI(resourceValue);
                }
            }

            foreach (var value in _resourceValuesUI)
            {
                value.UpdateDisplay();
            }
        }

        private bool TryGetResourceValue(ResourceValue resourceValue, out ResourceValueUI resourceValueUI)
        {
            foreach (var _resourceValueUI in _resourceValuesUI)
            {
                if (_resourceValueUI.ResourceValue != resourceValue) continue;
                resourceValueUI = _resourceValueUI;
                return true;
            }

            resourceValueUI = null;
            return false;
        }
        
        public override void OpenPanel()
        {
            base.OpenPanel();
            _market = MarketInteractor.SelectedMarket;
            UpdateDisplay();
            _panel.gameObject.SetActive(true);
        }

        public override void ClosePanel()
        {
            base.ClosePanel();
            _market = null;
            _panel.gameObject.SetActive(false);
        }
    }
}