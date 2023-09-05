using System;
using System.Collections.Generic;
using ResourceSystem;
using ResourceSystem.Global;
using UnityEngine;

namespace BuildingSystem.Facilities.FacilityTypes
{
    public class HouseFacility : FacilityType
    {
        private IResourceBorrower _selfBorrower => this;

        private static ResourceType s_populationResource;
        private static ResourceType s_workforceResource;
        private static ResourceType s_habitationResource;
        private static ResourceSlider s_mainPopulationSlider;
        private static ResourceSlider s_mainWorkForceSlider;

        public int inhabitants => (int)_selfBorrower.GetBorrowedQuantity(s_populationResource);

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
            
            s_mainPopulationSlider ??= GlobalResourceDeck.deck.GetSlider(s_populationResource);
            s_mainWorkForceSlider ??= GlobalResourceDeck.deck.GetSlider(s_workforceResource);
            
            loaners.Add(s_mainPopulationSlider,0);
            loaners.Add(s_mainWorkForceSlider,0);
        }

        private void OnDisable()
        {
            _selfBorrower.ReturnResourceAll();
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
            _selfBorrower.BorrowResource(maxPopulationCapacity - inhabitants, s_mainPopulationSlider);
        }

        /*
        public override ResourceDelta[] GetResourceDelta()
        {
            //print(name + _selfBorrower.GetBorrowedQuantity(s_populationResource));
            
            return new ResourceDelta[]
            {
                new ResourceDelta(s_workforceResource, quantityDelta: workForceRatio * inhabitants),
                new ResourceDelta(s_habitationResource, quantityDelta: maxPopulationCapacity)
            };
        }
        */
    }
}