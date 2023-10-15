using System;
using System.Collections.Generic;
using ResourceSystem;
using ResourceSystem.Markets;
using ResourceSystem.Productions;
using TerrainSystem;
using UnityEngine;

namespace BuildingSystem.Facilities.FacilityTypes
{
    public class TreeFacility : ProducerFacility
    {

        public override bool CanBePlaced(TerrainType terrain, out string conditionsFormat)
        {
            bool terrainCondition = terrain.id == "terrain_forest";

            FormatCondition("terrainCondition", terrainCondition);

            conditionsFormat = _placementConditions.GetLocalizedString();
            return terrainCondition;
        }
    }
}