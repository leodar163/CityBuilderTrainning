﻿using System.Collections.Generic;
using GridSystem;
using ResourceSystem;
using ResourceSystem.Markets;
using TimeSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace BuildingSystem.Facilities
{
    public class ConstructionSite : FacilityType, IEconomicActor
    {
        public Market market => cell.market;

        List<ResourceOrder> IEconomicActor.orders { get; } = new();

        public IEconomicActor economicActorSelf => this;

        private static ResourceType s_constructionForceResource;

        private FacilityType _facilityToBuild;
        public FacilityType facilityToBuild => _facilityToBuild;
        
        private float _constructionInvestment;
        private float _constructionProgression;
        public float constructionInvestment => _constructionInvestment;
        public float constructionProgression => _constructionProgression;

        private void Awake()
        {
            s_constructionForceResource = ResourceSet.Default.GetResource("resource_constructionforce");
        }

        public override void OnAddedToCell(CellData cellAddedTo)
        {
            base.OnAddedToCell(cellAddedTo);
            TimeManager.onMonthBegins += AddBuildProgression;
            float monthlyInvestment = Mathf.Clamp(constructionCost - _constructionInvestment, 0,
                FacilityPlacer.maxConstructionForceInvestment);

            economicActorSelf.SetOrder(s_constructionForceResource, monthlyInvestment, OrderType.Demand);
        }

        public override void OnRemovedFromCell(CellData cellRemovedFrom)
        {
            TimeManager.onMonthBegins -= AddBuildProgression;
            economicActorSelf.RemoveAllOrders();
            base.OnRemovedFromCell(cellRemovedFrom);
        }

        private void AddBuildProgression()
        {
            if (constructionCost <= 0)
            {
                BuildFacility();
                return;
            }
            
            float monthlyInvestment = Mathf.Clamp(constructionCost - _constructionInvestment, 0,
                FacilityPlacer.maxConstructionForceInvestment);

            economicActorSelf.SetOrder(s_constructionForceResource, monthlyInvestment, OrderType.Demand);

            _constructionInvestment += monthlyInvestment * cell.market.GetResourceAvailability(s_constructionForceResource);
            _constructionProgression = _constructionInvestment / constructionCost;
            
            if (_constructionProgression >= 1)
                BuildFacility();
        }

        private void BuildFacility()
        {
            if (Instantiate(facilityToBuild).TryGetComponent(out _facilityToBuild))
            {
                cell.maxFacilityCount++;
                cell.TryAddFacility(facilityToBuild);
                _facilityToBuild.transform.position = transform.position;
                _facilityToBuild.transform.rotation = transform.rotation;
                cell.maxFacilityCount--;
                cell.RemoveFacility(this);
            }
        }

        public void SetFacilityToBuild(FacilityType facility)
        {
            if (facility == null || facility == facilityToBuild) return;

            _facilityToBuild = facility;
            constructionCost = facilityToBuild.constructionCost;
        }
    }
}