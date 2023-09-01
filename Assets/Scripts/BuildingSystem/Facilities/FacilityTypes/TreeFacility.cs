using System;
using System.Collections.Generic;
using ResourceSystem;
using TerrainSystem;
using UnityEngine;

namespace BuildingSystem.Facilities.FacilityTypes
{
    public class TreeFacility : Facility
    {
        private static ResourceType environmentResource;

        private void Awake()
        {
            if (!environmentResource) environmentResource = ResourceSet.Default.GetResource("resource_environment");
        }

        public override bool CanBePlaced(TerrainType terrain, out string conditionsFormat)
        {
            bool terrainCondition = terrain is Forest;

            FormatCondition("terrainCondition", terrainCondition);

            conditionsFormat = _placementConditions.GetLocalizedString();
            return terrainCondition;
        }

        public override ResourceDelta[] GetResourceDelta()
        {
            return new ResourceDelta[]
            {
                new ResourceDelta(environmentResource, 0.3f)
            };
        }
    }
}