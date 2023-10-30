using System.Collections.Generic;
using TimeSystem;

namespace ResourceSystem.Markets.PeopleManagement
{
    public class PeopleNeedManager : IEconomicActor
    {
        public Market market { get; set; }
        List<ResourceOrder> IEconomicActor.orders { get; } = new();
        
        public IEconomicActor economicActorSelf => this;

        private static ResourceType s_popResource;
        private static ResourceType s_foodResource;
        private static ResourceType s_habitationResource;

        private float _unrest;
        public float unrest => _unrest;

        private float _unrestDelta;
        public float unrestDelta => _unrestDelta;
        
        private float _unrestDeltaFromFood;
        public float unrestDeltaFromFood => _unrestDeltaFromFood;
        
        
        private float _unrestDeltaFromHabitation;
        public float unrestDeltaFromHabitation => _unrestDeltaFromHabitation;
        
        public PeopleNeedManager(Market market)
        {
            s_popResource ??= ResourceSet.Default.GetResource("resource_population");
            s_foodResource ??= ResourceSet.Default.GetResource("resource_food");
            s_habitationResource ??= ResourceSet.Default.GetResource("resource_habitation");

            this.market = market;
            
            economicActorSelf.SetOrder(s_popResource, 10, OrderType.Offer);
            
            AdjustNeeds();

            TimeManager.onMonthEnds += AdjustNeeds;
        }

        private void AdjustNeeds()
        {
            if (market == null) return;

            float peopleAmount = market.GetResourceValueAmount(s_popResource, OrderType.Offer);
            
            economicActorSelf.SetOrder(s_foodResource, peopleAmount, OrderType.Demand);
            economicActorSelf.SetOrder(s_habitationResource, peopleAmount, OrderType.Demand);
            
            AdjustUnrest();
        }

        public void OnRemoved()
        {
            TimeManager.onMonthEnds -= AdjustNeeds;
            economicActorSelf.SetOrder(s_popResource, 0, OrderType.Offer);
            economicActorSelf.SetOrder(s_foodResource, 0, OrderType.Demand);
            economicActorSelf.SetOrder(s_habitationResource, 0, OrderType.Demand);
        }

        private void AdjustUnrest()
        {
            _unrestDeltaFromHabitation = (1 -  market.GetResourceAvailability(s_habitationResource)) / 2;
            _unrestDeltaFromFood = (1 - market.GetResourceAvailability(s_foodResource)) / 2 ;

            _unrestDelta = _unrestDeltaFromHabitation + unrestDeltaFromFood;

            _unrest += _unrestDelta / 100;
        }
    }
}