using Format;
using GridSystem;
using UnityEngine;

namespace Conditions.Placement
{
    [CreateAssetMenu(fileName = "NewEcosystemCondition", menuName = "Conditions/Placement Conditions/Ecosystem Condition")]
    public class EcosystemPlacementCondition : PlacementCondition
    {
        public override bool CanPlace(CellData cell, out string format, int indentLevel = 0)
        {
            bool condition = cell.market.isEcosystem;
            condition = inverseCondition ? !condition : condition;
            
            format = GetNeutralPlacementFormat(indentLevel) +
                     $" ({(condition ? FormatManager.conditionTrueFormat : FormatManager.conditionFalseFormat)})";
            return condition;
        }

        public override string GetNeutralPlacementFormat(int indentLevel = 0)
        {
            return $"<indent={indentLevel}em>" + string.Format(_localizedCondition.GetLocalizedString(),
                inverseCondition ? FormatManager.isNotFormat : FormatManager.isFormat);
        }
    }
}