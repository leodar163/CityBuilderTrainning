using Format;
using GridSystem;
using TerrainSystem;
using UnityEngine;

namespace Conditions.Placement
{
    [CreateAssetMenu(fileName = "NewTerrainCondition", menuName = "Conditions/Placement Conditions/Terrain Condition")]
    public class TerrainPlacementCondition : PlacementCondition
    {
        public TerrainType terrainFilter;

        public override bool CanPlace(CellData cell, out string format)
        {
            format = string.Format(_localizedCondition.GetLocalizedString(), 
                inverseCondition ? FormatManager.isNotFormat : FormatManager.isFormat , terrainFilter.terrainName);
            return inverseCondition ? cell.terrain != terrainFilter : cell.terrain == terrainFilter;
        }
    }
}