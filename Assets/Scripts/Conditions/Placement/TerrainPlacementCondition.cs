using Format;
using GridSystem;
using TerrainSystem.Scriptables;
using UnityEngine;

namespace Conditions.Placement
{
    [CreateAssetMenu(fileName = "NewTerrainCondition", menuName = "Conditions/Placement Conditions/Terrain Condition")]
    public class TerrainPlacementCondition : PlacementCondition
    {
        public ScriptableTerrain terrainFilter;

        public override bool CanPlace(CellData cell, out string format, int indentLevel = 0)
        {
            bool condition = inverseCondition ? cell.terrain != terrainFilter.terrain : cell.terrain == terrainFilter.terrain ;
            format = GetNeutralPlacementFormat(indentLevel) + 
                     $" ({(condition ? FormatManager.conditionTrueFormat : FormatManager.conditionFalseFormat)})";
            
            return condition;
        }

        public override string GetNeutralPlacementFormat(int indentLevel = 0)
        {
            return $"<indent={indentLevel}em>" + string.Format(_localizedCondition.GetLocalizedString(),
                inverseCondition
                    ? FormatManager.isNotFormat
                    : FormatManager.isFormat, terrainFilter.terrain.terrainName);
        }
    }
}