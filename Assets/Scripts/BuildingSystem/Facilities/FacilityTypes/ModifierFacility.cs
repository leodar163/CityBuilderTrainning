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

        #region CONSTRUCTORS

        public ModifierFacility(ModifierFacility template) : base(template)
        {
            _modifierTemplate = template._modifierTemplate;
        }
        
        public override FacilityType Copy()
        {
            return new ModifierFacility(this);
        }

        #endregion
        
    }
}