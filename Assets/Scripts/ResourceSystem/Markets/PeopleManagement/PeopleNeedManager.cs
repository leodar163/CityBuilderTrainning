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
        }

        public void OnRemoved()
        {
            TimeManager.onMonthEnds -= AdjustNeeds;
            economicActorSelf.SetOrder(s_popResource, 0, OrderType.Offer);
        }
    }
}