using System;
using System.Collections.Generic;
using BuildingSystem.Facilities;
using BuildingSystem.Facilities.FacilityTypes;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace ResourceSystem.Player
{
    public class PlayerResourceDeck : Singleton<PlayerResourceDeck>
    {
        [SerializeField] private ScriptableResourceDeck _resourceDeckTemplate;
        public readonly List<HouseFacility> houses = new();

        public static ResourceDeck deck { get; private set; }

        [SerializeField] private ResourceType populationResource;
        [SerializeField] private ResourceType workForceResource;
        [SerializeField] private ResourceType habitationResource;

        private ResourceSlider populationSlider;
        private ResourceSlider workforceSlider;
        private ResourceSlider habitationSlider;

        private void Awake()
        {
            deck = _resourceDeckTemplate.GetResourceDeckCopy();
            
            if (!populationResource) populationResource = ResourceSet.Default.GetResource("resource_population");
            if (!workForceResource) workForceResource = ResourceSet.Default.GetResource("resource_workforce");
            if (!habitationResource) habitationResource = ResourceSet.Default.GetResource("resource_habitation");

            populationSlider = deck.GetSlider(populationResource);
            workforceSlider = deck.GetSlider(workForceResource);
            habitationSlider = deck.GetSlider(habitationResource);
            
            Facility.onFacilityBuild += RecordHouse;
            Facility.onFacilityDestroyed += ForgetHouse;
        }
        
        
        private void Update()
        {
            float workforce = 0;
            float habitations = 0;
            
            foreach (var house in houses)
            {
                workforce += house.producedWorkForce;
                habitations += house.maxPopulationCapacity;
            }

            workforceSlider.maxQuantity = workforce;
            workforceSlider.quantity = workforce;

            habitationSlider.maxQuantity = habitations;
            habitationSlider.quantity = habitations;
        }

        private void RecordHouse(Facility facilityBuilt)
        {
            if (facilityBuilt is HouseFacility house)
            {
                houses.Add(house);
            }
        }

        private void ForgetHouse(Facility facilityDestroyed)
        {
            if (facilityDestroyed is HouseFacility house && houses.Contains(house))
            {
                houses.Remove(house);
            }
        }
    }
}