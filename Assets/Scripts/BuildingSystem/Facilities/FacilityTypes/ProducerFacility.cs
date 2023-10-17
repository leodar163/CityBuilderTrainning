using System;
using System.Collections.Generic;
using GridSystem;
using ResourceSystem.Markets;
using ResourceSystem.Productions;
using TimeSystem;
using UnityEngine;

namespace BuildingSystem.Facilities.FacilityTypes
{
    public class ProducerFacility : FacilityType, IProducer
    {
        public IEconomicActor economicActorSelf => this;
        public Market market => cell.market;
        List<ResourceOrder> IEconomicActor.orders { get; } = new();

        public IProducer producerSelf => this;
        [Header("Production")]
        [SerializeField] private List<ProductionLine> _productionLines;
        public List<ProductionLine> productionLines => _productionLines;

        public override void OnAddedToCell(CellData cellAddedTo)
        {
            base.OnAddedToCell(cellAddedTo);
            producerSelf.FetchResources();
            producerSelf.Produce();
        }

        protected virtual void OnEnable()
        {
            TimeManager.onNewMonth += producerSelf.FetchResources;
            TimeManager.onMonthEnds += producerSelf.Produce;
        }

        protected void OnDisable()
        {
            TimeManager.onNewMonth -= producerSelf.FetchResources;
            TimeManager.onMonthEnds -= producerSelf.Produce;
            
            economicActorSelf.RemoveAllOrders();
        }
    }
}