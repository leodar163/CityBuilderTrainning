using System;
using System.Collections.Generic;
using GridSystem;
using ResourceSystem.Markets;
using ResourceSystem.Productions;
using TimeSystem;
using UnityEngine;

namespace BuildingSystem.Facilities.FacilityTypes
{
    [Serializable]
    public class ProducerFacility : FacilityType, IProducer
    {
        public IEconomicActor economicActorSelf => this;
        public string EconomicActorName => facilityName;
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
            
            TimeManager.onNewMonth += producerSelf.FetchResources;
            TimeManager.onMonthEnds += producerSelf.Produce;
        }

        public override void OnRemovedFromCell(CellData cellRemovedFrom)
        {
            base.OnRemovedFromCell(cellRemovedFrom);
            
            TimeManager.onNewMonth -= producerSelf.FetchResources;
            TimeManager.onMonthEnds -= producerSelf.Produce;
            
            economicActorSelf.RemoveAllOrders();
        }

        public override FacilityType Copy()
        {
            return new ProducerFacility()
            {
                _renderData = _renderData,
                _scaleMultiplier = _scaleMultiplier,
                _facilityName = _facilityName,
                _facilityDescription = _facilityDescription,
                _placementCondition = _placementCondition,
                constructionCost = constructionCost,
                _sizeRadius = _sizeRadius,
                _productionLines = new List<ProductionLine>(_productionLines)
            };
        }
    }
}