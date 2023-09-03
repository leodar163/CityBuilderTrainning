using System;
using System.Collections.Generic;
using BuildingSystem.Facilities;
using BuildingSystem.Facilities.FacilityTypes;
using TimeSystem;
using UnityEngine;
using Utils;

namespace ResourceSystem.Global
{
    public class GlobalResourceDeck : Singleton<GlobalResourceDeck>
    {
        [SerializeField] private ScriptableResourceDeck _resourceDeckTemplate;
        public readonly List<HouseFacility> houses = new();

        private ResourceDeck _deck;
        public static ResourceDeck deck => Instance._deck;

            [SerializeField] private ResourceType populationResource;
        [SerializeField] private ResourceType workForceResource;
        [SerializeField] private ResourceType habitationResource;

        private ResourceSlider populationSlider;
        private ResourceSlider workforceSlider;
        private ResourceSlider habitationSlider;

        private void Awake()
        {
            _deck = _resourceDeckTemplate.GetResourceDeckCopy();
            
            if (!populationResource) populationResource = ResourceSet.Default.GetResource("resource_population");
            if (!workForceResource) workForceResource = ResourceSet.Default.GetResource("resource_workforce");
            if (!habitationResource) habitationResource = ResourceSet.Default.GetResource("resource_habitation");

            populationSlider = deck.GetSlider(populationResource);
            workforceSlider = deck.GetSlider(workForceResource);
            habitationSlider = deck.GetSlider(habitationResource);
            
            Facility.onFacilityBuild += RecordHouse;
            Facility.onFacilityDestroyed += ForgetHouse;

            TimeManager.onNewMonth += _ => deck.ApplyDeltaToSliders();
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

        private void RecordHouse(Facility facilityBuilt)
        {
            if (facilityBuilt is HouseFacility house && !houses.Contains(house))
            {
                houses.Add(house);
                habitationSlider.Sub(house);
                workforceSlider.Sub(house);
            }
        }

        private void ForgetHouse(Facility facilityDestroyed)
        {
            if (facilityDestroyed is HouseFacility house && houses.Contains(house))
            {
                houses.Remove(house);
                habitationSlider.Unsub(house);
                workforceSlider.Unsub(house);
            }
        }
    }
}