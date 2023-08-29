using System.Collections.Generic;
using ResourceSystem;
using TerrainSystem;

namespace BuildingSystem.Facilities.FacilityTypes
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

        public override List<ResourceQuantity> GetResourceDelta()
        {
            List<ResourceQuantity> deltas = new()
            {
                new ResourceQuantity(ResourceSet.Default.GetResource("resource_environment"), 0.3f)
            };

            return deltas;
        }
    }
}