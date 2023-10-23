using System.Collections.Generic;
using System.Globalization;
using Format;
using ResourceSystem.Markets;
using ResourceSystem.UI;
using TMPro;
using UnityEngine;

namespace ResourceSystem.Productions.UI
{
    public class ProductionLineUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _inLayout;
        [SerializeField] private RectTransform _outLayout;
        [SerializeField] private TextMeshProUGUI _productionLineName;
        [SerializeField] private TextMeshProUGUI _efficiency;
        [Space]
        [SerializeField] private ResourceQuantityUI resourceQuantityUITemplate;

        private readonly List<ResourceQuantityUI> _demands = new();
        private readonly List<ResourceQuantityUI> _offers = new();

        private ProductionLine _productionLine;
        private Market _market;

        public void SetProductionLine(ProductionLine productionLine, Market market = null)
        {
            if (_productionLine != null && productionLine == _productionLine) return;

            _market = market;
            _productionLine = productionLine;
            DisplayProductionLine();
        }

        private void DisplayProductionLine()
        {
            _productionLineName.SetText(_productionLine.name);
            
            Color efficiencyColor = _productionLine.efficiency < 1
                ? FormatManager.negativeColor
                : FormatManager.DefaultTextColor;

            _efficiency.color = efficiencyColor;
            _efficiency.SetText(_market == null ? "" :
                (Mathf.RoundToInt(_productionLine.efficiency * 10000) / 100).ToString(CultureInfo.CurrentCulture) +
                "%");
            
            DisplayResourceQuantities();
        }

        private void DisplayResourceQuantities()
        {
            if (_productionLine == null) return;
            
            for (int i = 0; i < _productionLine.demands.Count; i++)
            {
                ResourceQuantityUI resourceQuantityUI;
                
                if (_demands.Count < i + 1)
                {
                    if (_offers.Count > _productionLine.offers.Count)
                    {
                        resourceQuantityUI = _offers[^1];
                        _offers.Remove(resourceQuantityUI);
                        _demands.Add(resourceQuantityUI);
                        resourceQuantityUI.transform.SetParent(_inLayout);
                    }
                    else
                    {
                        if (!Instantiate(resourceQuantityUITemplate, _inLayout).TryGetComponent(out resourceQuantityUI))
                            return;

                        _demands.Add(resourceQuantityUI);
                    }
                }
                else
                {
                    resourceQuantityUI = _demands[i];
                }

                Color quantityColor = _market == null ||
                                      _market.GetResourceAvailability(_demands[i].resource) > 1
                    ? FormatManager.DefaultTextColor
                    : FormatManager.negativeColor;

                resourceQuantityUI.DisplayResourceQuantity(_productionLine.demands[i].resource,
                        _productionLine.demands[i].quantity, quantityColor);
            }

            for (int i = 0; i < _productionLine.offers.Count; i++)
            {
                ResourceQuantityUI resourceQuantityUI;

                if (_offers.Count < i + 1)
                {
                    if (_demands.Count > _productionLine.demands.Count)
                    {
                        resourceQuantityUI = _demands[^1];
                        _demands.Remove(resourceQuantityUI);
                        _offers.Add(resourceQuantityUI);

                        resourceQuantityUI.transform.SetParent(_outLayout);
                    }
                    else
                    {
                        if (!Instantiate(resourceQuantityUITemplate, _outLayout).TryGetComponent(out resourceQuantityUI))
                            return;

                        _offers.Add(resourceQuantityUI);
                    }
                }
                else
                {
                    resourceQuantityUI = _offers[i];
                }

                Color quantityColor = _market == null || _productionLine.efficiency >= 1
                    ? FormatManager.DefaultTextColor
                    : FormatManager.negativeColor;

                resourceQuantityUI.DisplayResourceQuantity(_productionLine.offers[i].resource,
                    _productionLine.offers[i].quantity * _productionLine.efficiency, quantityColor);
            }

            while (_demands.Count > _productionLine.demands.Count)
            {
                _demands.Remove(_demands[^1]);
            }

            while (_offers.Count > _productionLine.offers.Count)
            {
                _offers.Remove(_offers[^1]);
            }
        }
    }
}