using Format;
using GridSystem;
using UnityEngine;
using UnityEngine.Localization;

namespace Conditions.Placement.Flow
{
    [CreateAssetMenu(fileName = "NewPlacementConditionList", menuName = "Conditions/Placement Conditions/Placement Condition List")]
    public class PlacementConditionList : PlacementCondition
    {
        public int nbrOfNeededConditions;
        public bool allConditions;
        [Space]
        [SerializeField] private LocalizedString _localizedMultipleConditions;
        [SerializeField] private LocalizedString _localizedAllConditions;
        [Space]
        public PlacementCondition[] placementConditions;
        

        public override bool CanPlace(CellData cell, out string format, int indentLevel = 0)
        {
            bool plural = allConditions || nbrOfNeededConditions > 1;
            
            string conditionsFormat = "";
            int neededConditions = allConditions ? placementConditions.Length : nbrOfNeededConditions;
            
            for (int i = 0; i < placementConditions.Length; i++)
            {
                conditionsFormat += '\n';
                
                PlacementCondition condition = placementConditions[i];

                if (condition.CanPlace(cell, out string conditionFormat, indentLevel + 1))
                {
                    if (!inverseCondition)
                        neededConditions--;
                }
                else
                {
                    if (inverseCondition)
                        neededConditions--;
                }

                conditionsFormat += conditionFormat;
            }

            string isCondition =
                plural ? inverseCondition ? FormatManager.isNotFormatPlural : FormatManager.isFormatPlural :
                inverseCondition ? FormatManager.isNotFormat : FormatManager.isFormat;

            format = $"<indent={indentLevel}em>";
            
            if (allConditions)
            {
                format += string.Format(_localizedAllConditions.GetLocalizedString(), isCondition);
            }
            else
            {
                format += string.Format(_localizedMultipleConditions.GetLocalizedString(), nbrOfNeededConditions,
                    isCondition, plural ? "s" : "");
            }

            format +=
                $" ({(neededConditions <= 0 ? FormatManager.conditionTrueFormat : FormatManager.conditionFalseFormat)})"
                + conditionsFormat;

            return neededConditions <= 0;
        }

        public override string GetNeutralPlacementFormat(int indentLevel = 0)
        {
            string format = $"<indent={indentLevel}em>";
            
            bool plural = allConditions || nbrOfNeededConditions > 1;
            
            string isCondition =
                plural ? inverseCondition ? FormatManager.isNotFormatPlural : FormatManager.isFormatPlural :
                inverseCondition ? FormatManager.isNotFormat : FormatManager.isFormat;
            
            if (allConditions)
            {
                format += string.Format(_localizedAllConditions.GetLocalizedString(), isCondition);
            }
            else
            {
                format += string.Format(_localizedMultipleConditions.GetLocalizedString(), nbrOfNeededConditions,
                    isCondition, plural ? "s" : "");
            }
            
            for (int i = 0; i < placementConditions.Length; i++)
            {
                format += '\n';
                
                PlacementCondition condition = placementConditions[i];

                format += condition.GetNeutralPlacementFormat(indentLevel + 1);
            }

            return format;
        }
    }
}