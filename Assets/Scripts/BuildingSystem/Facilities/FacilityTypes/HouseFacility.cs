using System;
using System.Collections.Generic;
using ResourceSystem;
using ResourceSystem.Player;
using UnityEngine;

namespace BuildingSystem.Facilities.FacilityTypes
{
    public class HouseFacility : Facility, IResourceBorrower
    {
        public Dictionary<ResourceSlider, float> borrowedResources { get; } = new();


        private static ResourceType s_populationResource;
        private static ResourceSlider s_mainPopulationSlider;

        public int populationSettled => (int)borrowedResources[s_mainPopulationSlider];

        public int maxPopulationCapacity = 4;
        public float workForceRatio = 1;

        private void Awake()
        {
            if (!s_populationResource) 
                s_populationResource = ResourceSet.Default.GetResource("resource_population");
            s_mainPopulationSlider ??= PlayerResourceDeck.deck.GetSlider(s_populationResource);
            
            borrowedResources.Add(s_mainPopulationSlider,0);
        }

        private void OnDisable()
        {
            IResourceBorrower selfBorrower = this;
            selfBorrower.ReleaseResource(populationSettled, s_mainPopulationSlider);
        }

        private void Update()
        {
            if (populationSettled < maxPopulationCapacity)
            {
                BorrowResource(maxPopulationCapacity - populationSettled, s_populationResource);
            }
        }

        public void BorrowResource(float quantity, ResourceType resource)
        {
            ResourceSlider sliderToBorrowTo = null;
            
            if (resource == s_populationResource)
            {
                sliderToBorrowTo = s_mainPopulationSlider;
            }

            IResourceBorrower selfBorrower = this;
            selfBorrower.BorrowResource(quantity, sliderToBorrowTo);
        }

        public void ReleaseResource(float quantityToRelease, ResourceType resource)
        {
            List<ResourceSlider> slidersToRemove = new();
            
            foreach (var pair in borrowedResources)
            {
                if (pair.Key.resource == resource)
                {
                    float quantityReleasable = quantityToRelease > pair.Value ? pair.Value : quantityToRelease;
                    IResourceBorrower selfBorrower = this;
                    selfBorrower.ReleaseResource(quantityReleasable, pair.Key);
                    
                    if(pair.Value == 0)
                        slidersToRemove.Add(pair.Key);
                }
            }

            foreach (var slider in slidersToRemove)
            {
                borrowedResources.Remove(slider);
            }
        }
    }
}