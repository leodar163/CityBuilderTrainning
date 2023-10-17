using Format;
using GridSystem;
using UnityEngine;
using UnityEngine.Localization;

namespace Conditions.Placement
{
    public class EcosystemPlacementCondition : PlacementCondition
    {
        [SerializeField] private LocalizedString _localizedEcosystemCondition;

        public override bool CanPlace(CellData cell, out string format)
        {
            bool condition = cell.market.isEcosystem;
            condition = inverseCondition ? !condition : condition;
            
            format = string.Format(_localizedEcosystemCondition.GetLocalizedString(),
                inverseCondition ? FormatManager.isNotFormat : FormatManager.isFormat) +
                     $" : {(condition ? FormatManager.conditionTrueFormat : FormatManager.conditionFalseFormat)}";
            return condition;
        }
    }
}