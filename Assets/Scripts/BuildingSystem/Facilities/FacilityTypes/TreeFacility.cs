using System;
using System.Collections.Generic;
using ResourceSystem;
using TerrainSystem;
using UnityEngine;

namespace BuildingSystem.Facilities.FacilityTypes
{
    public class TreeFacility : FacilityType
    {
        private static ResourceType environmentResource;

        protected override void Awake()
        {
            base.Awake();
            if (!environmentResource) environmentResource = ResourceSet.Default.GetResource("resource_environment");
        }

        public override bool CanBePlaced(TerrainType terrain, out string conditionsFormat)
        {
            bool terrainCondition = terrain is Forest;

            FormatCondition("terrainCondition", terrainCondition);

            conditionsFormat = _placementConditions.GetLocalizedString();
            return terrainCondition;
        }
    }
}