using System;
using System.Collections.Generic;
using ResourceSystem;
using ResourceSystem.Player;
using UnityEngine;

namespace BuildingSystem.Facilities.FacilityTypes
{
    public class HouseFacility : Facility, IResourceBorrower
    {
        public Dictionary<ResourceSlider, float> loaners { get; } = new();
        public IResourceBorrower selfBorrower => this;

        private static ResourceType s_populationResource;
        private static ResourceType s_workforceResource;
        private static ResourceType s_habitationResource;
        private static ResourceSlider s_mainPopulationSlider;
        private static ResourceSlider s_mainWorkForceSlider;

        public int inhabitants => (int)selfBorrower.GetBorrowedQuantity(s_populationResource);

        public int maxPopulationCapacity = 4;
        public float workForceRatio = 1;

        public float producedWorkForce => inhabitants * workForceRatio;
        
        private void Awake()
        {
            if (!s_populationResource) 
                s_populationResource = ResourceSet.Default.GetResource("resource_population");
            if (!s_workforceResource) 
                s_workforceResource = ResourceSet.Default.GetResource("resource_workforce");
            if (!s_habitationResource) 
                s_habitationResource = ResourceSet.Default.GetResource("resource_habitation");
            
            s_mainPopulationSlider ??= PlayerResourceDeck.deck.GetSlider(s_populationResource);
            s_mainWorkForceSlider ??= PlayerResourceDeck.deck.GetSlider(s_workforceResource);
            
            loaners.Add(s_mainPopulationSlider,0);
            loaners.Add(s_mainWorkForceSlider,0);
        }

        private void OnDisable()
        {
            selfBorrower.ReturnResource(inhabitants, s_mainPopulationSlider);
        }

        private void Update()
        {
            if (inhabitants < maxPopulationCapacity)
            {
                BorrowPopulation();
            }
        }

        private void BorrowPopulation()
        {
            selfBorrower.BorrowResource(maxPopulationCapacity - inhabitants, s_mainPopulationSlider);
        }

        public override List<ResourceDelta> GetResourceDelta()
        {
            return new List<ResourceDelta>
            {
                new ResourceDelta(s_workforceResource, quantityDelta: workForceRatio * inhabitants)
            };
        }
    }
}