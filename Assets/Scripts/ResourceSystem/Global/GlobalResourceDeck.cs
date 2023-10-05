using System;
using System.Collections.Generic;
using BuildingSystem.Facilities;
using BuildingSystem.Facilities.FacilityTypes;
using ResourceSystem.Scriptables;
using ResourceSystem.Transactions;
using ResourceSystem.Transactions.UI;
using TimeSystem;
using TMPro;
using UnityEngine;
using Utils;

namespace ResourceSystem.Global
{
    public class GlobalResourceDeck : Singleton<GlobalResourceDeck>, ITransactor
    {
        [SerializeField] private ResourceDeck _resourceDeckTemplate;
        private readonly List<HouseFacility> _houses = new();

        public float habitationNbr { get; private set; }
        public float workforceAmount { get; private set; }
        [SerializeField] private ResourceContainerUI _populationSign;
        
        private static ResourceContainer s_popContainer;

        private static ResourceType s_popResource;
        private static ResourceType s_habitationResource;
        private static ResourceType s_wfResource;
        
        List<ResourceContainer> ITransactor.registry { get; } = new();
        public ITransactor transactorSelf => this;

        private void Awake()
        {
            s_popResource = ResourceSet.Default.GetResource("resource_population");
            s_habitationResource = ResourceSet.Default.GetResource("resource_habitation");
            s_wfResource = ResourceSet.Default.GetResource("resource_workforce");
            
            transactorSelf.InitContainers(_resourceDeckTemplate);
            
            transactorSelf.TryGetContainer(s_popResource ,out s_popContainer);
            _populationSign.Container = s_popContainer;
            
            FacilityType.onFacilityBuild += RecordHouse;
            FacilityType.onFacilityDestroyed += ForgetHouse;
        }

        private void RecordHouse(FacilityType facilityTypeBuilt)
        {
            if (facilityTypeBuilt is HouseFacility house && !_houses.Contains(house))
            {
                _houses.Add(house);
            }
        }

        private void ForgetHouse(FacilityType facilityTypeDestroyed)
        {
            if (facilityTypeDestroyed is HouseFacility house && _houses.Contains(house))
            {
                _houses.Remove(house);
            }
        }
    }
}