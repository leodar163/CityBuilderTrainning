using System;
using GridSystem;
using ResourceSystem.Markets.Modifiers;
using UnityEngine;

namespace BuildingSystem.Facilities.FacilityTypes
{
    [Serializable]
    public class ModifierFacility : FacilityType
    {
        [SerializeField] private ScriptableMarketModifier _modifierTemplate;
        private MarketModifier _modifier;
        
        
        public override void OnAddedToCell(CellData cellAddedTo)
        {
            base.OnAddedToCell(cellAddedTo);

            _modifier = _modifierTemplate.GetEffectCopy() as MarketModifier;
            
            _modifier?.Apply(cellAddedTo.market);
        }

        public override void OnRemovedFromCell(CellData cellRemovedFrom)
        {
            base.OnRemovedFromCell(cellRemovedFrom);
            _modifier.Unapply();
            
        }

        public override FacilityType Copy()
        {
            return new ModifierFacility
            {
                _renderData = _renderData,
                _scaleMultiplier = _scaleMultiplier,
                _facilityName = _facilityName,
                _facilityDescription = _facilityDescription,
                _placementCondition = _placementCondition,
                constructionCost = constructionCost,
                _sizeRadius = _sizeRadius,
                
                _modifierTemplate = _modifierTemplate
            };
        }
    }
}