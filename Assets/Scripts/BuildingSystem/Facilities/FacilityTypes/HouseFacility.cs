using System;
using System.Collections.Generic;
using GridSystem;
using ResourceSystem;
using ResourceSystem.Global;
using ResourceSystem.Transactions;
using UnityEngine;

namespace BuildingSystem.Facilities.FacilityTypes
{
    public class HouseFacility : FacilityType
    {
        private static ResourceType s_populationResource;
        private static ResourceType s_workforceResource;
        private static ResourceType s_habitationResource;
        private ResourceContainer _popContainer;

        public int inhabitants => (int)_popContainer.totalQuantity;

        public int maxPopulationCapacity = 4;
        public float workForceRatio = 1;


        
        protected override void Awake()
        {
            base.Awake();
            
            if (!s_populationResource) 
                s_populationResource = ResourceSet.Default.GetResource("resource_population");
            if (!s_workforceResource) 
                s_workforceResource = ResourceSet.Default.GetResource("resource_workforce");
            if (!s_habitationResource) 
                s_habitationResource = ResourceSet.Default.GetResource("resource_habitation");
            
            /*
            s_mainPopulationSlider ??= GlobalResourceDeck.deck.GetSlider(s_populationResource);
            s_mainWorkForceSlider ??= GlobalResourceDeck.deck.GetSlider(s_workforceResource);
            */
        }

        public override void OnAddedToCell(CellData cellAddedTo)
        {
            base.OnAddedToCell(cellAddedTo);
            transactorSelf.TryGetContainer(s_populationResource,out _popContainer);
            producerSelf.AddCreditor(GlobalResourceDeck.Instance);
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