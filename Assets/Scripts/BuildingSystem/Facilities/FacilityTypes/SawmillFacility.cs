using System.Collections.Generic;
using ResourceSystem.Markets;
using ResourceSystem.Productions;
using UnityEngine;

namespace BuildingSystem.Facilities.FacilityTypes
{
    public class SawmillFacility : FacilityType, IProducer
    {
        List<ResourceOrder> IEconomicActor.orders { get; } = new();

        public IEconomicActor economicActorSelf => this;
        public IProducer producerSelf => this;

        [Header("Production")] 
        [SerializeField] private List<ProductionLine> _productionLines;
        public List<ProductionLine> productionLines => _productionLines;
    }
}