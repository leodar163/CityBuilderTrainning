using System.Collections.Generic;
using BuildingSystem.Facilities;
using BuildingSystem.Facilities.FacilityTypes;
using ResourceSystem.Productions.UI;
using TMPro;
using UnityEngine;

namespace ToolTipSystem.Messages
{
    public class FacilityTooltipMessageUI : TooltipMessageUI
    {
        [SerializeField] private TextMeshProUGUI _facilityName;
        [SerializeField] private TextMeshProUGUI _facilityDesc;
        [SerializeField] private TextMeshProUGUI _placementConditions;
        [Space] 
        [SerializeField] private RectTransform _productionLinesLayout;
        [SerializeField] private ProductionLineUI _productionLineUITemplate;
        
        private readonly List<ProductionLineUI> _productionLines = new();

        public void SetFacility(FacilityType facility)
        {
            _facilityName.SetText(facility.facilityName);
            
            _facilityDesc.SetText(facility.facilityDesc);
            
            if (facility is ProducerFacility producer)
            {
                DisplayProduction(producer);
            }
            else
            {
                _productionLinesLayout.gameObject.SetActive(false);
            }
            
            _placementConditions.SetText(facility.GetPlacementConditions());
        }

        private void DisplayProduction(ProducerFacility producer)
        {
            while (_productionLines.Count < producer.productionLines.Count)
            {
                if (Instantiate(_productionLineUITemplate.gameObject, _productionLinesLayout)
                    .TryGetComponent(out ProductionLineUI productionLineUI))
                {
                    _productionLines.Add(productionLineUI);
                }
            }

            while (_productionLines.Count > producer.productionLines.Count)
            {
                ProductionLineUI productionLineUI = _productionLines[^1];
                _productionLines.Remove(productionLineUI);
                Destroy(productionLineUI.gameObject);
            }

            for (int i = 0; i < _productionLines.Count; i++)
            {
               _productionLines[i].SetProductionLine(producer.productionLines[i], producer.cell?.market);
            }
            
            _productionLinesLayout.gameObject.SetActive(true);
        }
    }
}