using System;
using System.Collections.Generic;
using ResourceSystem;
using ResourceSystem.Markets;
using ResourceSystem.Productions;
using TerrainSystem;
using UnityEngine;

namespace BuildingSystem.Facilities.FacilityTypes
{
    public class TreeFacility : FacilityType, IProducer
    {
        List<ResourceOrder> IEconomicActor.orders { get; } = new();

        public IEconomicActor economicActorSelf => this;
        public IProducer producerSelf => this;

        [Header("Production")] 
        [SerializeField] private List<ProductionLine> _productionLines;
        public List<ProductionLine> productionLines => _productionLines;

        public override bool CanBePlaced(TerrainType terrain, out string conditionsFormat)
        {
            bool terrainCondition = terrain.id == "terrain_forest";

            FormatCondition("terrainCondition", terrainCondition);

            conditionsFormat = _placementConditions.GetLocalizedString();
            return terrainCondition;
        }
    }
}