using Format;
using GridSystem;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;

namespace Conditions.Placement.Flow
{
    [CreateAssetMenu(fileName = "NewPlacementConditionList", menuName = "")]
    public class PlacementConditionList : PlacementCondition
    {
        public int nbrOfNeededConditions;
        public bool allConditions;
        public PlacementCondition[] placementConditions;
        [SerializeField] private LocalizedString _localizedMultipleConditions;
        [SerializeField] private LocalizedString _localizedAllConditions;

        public override bool CanPlace(CellData cell, out string format)
        {
            bool plural = allConditions || nbrOfNeededConditions > 1;
            
            string conditionsFormat = "";
            int neededConditions = allConditions ? placementConditions.Length : nbrOfNeededConditions;
            
            for (int i = 0; i < placementConditions.Length; i++)
            {
                PlacementCondition condition = placementConditions[i];

                if (condition.CanPlace(cell, out string conditionFormat))
                {
                    if (!inverseCondition)
                        nbrOfNeededConditions--;
                }
                else
                {
                    if (inverseCondition)
                        nbrOfNeededConditions--;
                }

                conditionsFormat += conditionFormat;
                if (i < placementConditions.Length - 1) conditionsFormat += '\n';
            }

            string isCondition =
                plural ? inverseCondition ? FormatManager.isNotFormatPlural : FormatManager.isFormatPlural :
                inverseCondition ? FormatManager.isNotFormat : FormatManager.isFormat;

            if (allConditions)
            {
                format = string.Format(_localizedAllConditions.GetLocalizedString(), isCondition);
            }
            else
            {
                format = string.Format(_localizedMultipleConditions.GetLocalizedString(), isCondition,
                    nbrOfNeededConditions);
            }

            format +=
                $" : {(neededConditions <= 0 ? FormatManager.conditionTrueFormat : FormatManager.conditionFalseFormat)}\n{conditionsFormat}";

            return true;
        }
    }
}