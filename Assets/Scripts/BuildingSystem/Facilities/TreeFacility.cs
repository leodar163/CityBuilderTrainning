using TerrainSystem;

namespace BuildingSystem.Facilities
{
    public class TreeFacility : Facility
    {
        public override bool CanBePlaced(TerrainType terrain, out string conditionsFormat)
        {
            bool terrainCondition = terrain is Forest;

            FormatCondition("terrainCondition", terrainCondition);

            conditionsFormat = _placementConditions.GetLocalizedString();
            return terrainCondition;
        }
    }
}