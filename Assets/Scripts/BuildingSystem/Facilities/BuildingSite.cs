using System.Collections.Generic;
using GridSystem;
using ResourceSystem;
using ResourceSystem.Markets;
using TimeSystem;
using UnityEngine;

namespace BuildingSystem.Facilities
{
    public class BuildingSite : FacilityType, IEconomicActor
    {
        public Market market => cell.market;

        List<ResourceOrder> IEconomicActor.orders { get; } = new();

        public IEconomicActor economicActorSelf => this;

        private static ResourceType s_constructionForceResource;

        [SerializeField] private FacilityType _facilityToBuild;
        [SerializeField] private float _constructionCost;
        private float _constructionInvestment;

        private void Awake()
        {
            s_constructionForceResource = ResourceSet.Default.GetResource("resource_constructionforce");
        }

        public override void OnAddedToCell(CellData cellAddedTo)
        {
            base.OnAddedToCell(cellAddedTo);
            TimeManager.onMonthBegins += Build;
        }

        public override void OnRemovedFromCell(CellData cellRemovedFrom)
        {
            TimeManager.onMonthBegins -= Build;
            economicActorSelf.RemoveAllOrders();
            base.OnRemovedFromCell(cellRemovedFrom);
        }

        private void Build()
        {
            float monthlyInvestment = Mathf.Clamp(_constructionCost - _constructionInvestment, 0,
                FacilityPlacer.maxConstructionForceInvestment);
            
            if (monthlyInvestment == 0) OnBuilt();

            economicActorSelf.SetOrder(s_constructionForceResource, monthlyInvestment, OrderType.Demand);
        }

        private void OnBuilt()
        {
            if (Instantiate(_facilityToBuild, transform.position, transform.rotation).TryGetComponent(out _facilityToBuild))
            {
                _facilityToBuild.OnAddedToCell(cell);
            }
            
            OnRemovedFromCell(cell);
            //need to notify the cell ????
        }
    }
}