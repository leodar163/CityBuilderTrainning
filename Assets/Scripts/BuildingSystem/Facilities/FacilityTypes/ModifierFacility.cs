using GridSystem;
using ResourceSystem.Markets.Modifiers;
using UnityEngine;

namespace BuildingSystem.Facilities.FacilityTypes
{
    public class ModifierFacility : FacilityType
    {
        [SerializeField] private ScriptableMarketModifier _modifierTemplate;
        private MarketModifier _modifier;
        
        
        public override void OnAddedToCell(CellData cellAddedTo)
        {
            base.OnAddedToCell(cellAddedTo);

            _modifier = _modifierTemplate.GetEffectCopy();
            
            _modifier.Apply(cellAddedTo.market);
        }

        public override void OnRemovedFromCell(CellData cellRemovedFrom)
        {
            base.OnRemovedFromCell(cellRemovedFrom);
            _modifier.Unapply();
            
        }
    }
}