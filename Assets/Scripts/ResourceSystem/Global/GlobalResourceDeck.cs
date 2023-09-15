using System;
using System.Collections.Generic;
using BuildingSystem.Facilities;
using BuildingSystem.Facilities.FacilityTypes;
using ResourceSystem.Transactions;
using TimeSystem;
using UnityEngine;
using Utils;

namespace ResourceSystem.Global
{
    public class GlobalResourceDeck : Singleton<GlobalResourceDeck>, ITransactor
    {
        [SerializeField] private ScriptableResourceDeck _resourceDeckTemplate;
        public readonly List<HouseFacility> houses = new();

        private ResourceSlider populationSlider;
        private ResourceSlider workforceSlider;
        private ResourceSlider habitationSlider;

        List<ResourceContainer> ITransactor.registry { get; } = new();
        public ITransactor transactorSelf => this;
        

        private void Awake()
        {
            transactorSelf.InitContainers(_resourceDeckTemplate);
            

            
            FacilityType.onFacilityBuild += RecordHouse;
            FacilityType.onFacilityDestroyed += ForgetHouse;
        }
        
        
        private void Update()
        {
            //float workforce = 0;
            //float habitations = 0;
            
            //foreach (var house in houses)
            {
               // workforce += house.producedWorkForce;
                //habitations += house.maxPopulationCapacity;
            }

            //workforceSlider.maxQuantity = workforce;
            //workforceSlider.nativeQuantity = workforce;

            //habitationSlider.maxQuantity = habitations;
            //habitationSlider.nativeQuantity = habitations;
        }

        private void RecordHouse(FacilityType facilityTypeBuilt)
        {
            if (facilityTypeBuilt is HouseFacility house && !houses.Contains(house))
            {
                houses.Add(house);
                habitationSlider.Sub(house);
                workforceSlider.Sub(house);
            }
        }

        private void ForgetHouse(FacilityType facilityTypeDestroyed)
        {
            if (facilityTypeDestroyed is HouseFacility house && houses.Contains(house))
            {
                houses.Remove(house);
                habitationSlider.Unsub(house);
                workforceSlider.Unsub(house);
            }
        }
    }
}