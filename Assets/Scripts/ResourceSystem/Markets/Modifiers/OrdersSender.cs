using System.Collections.Generic;
using UnityEngine.Localization;

namespace ResourceSystem.Markets.Modifiers
{
    public class OrdersSender : IEconomicActor
    {
        private readonly LocalizedString _name;
        public string EconomicActorName => _name.GetLocalizedString();
        public Market market { get; }

        List<ResourceOrder> IEconomicActor.orders { get; } = new();

        public IEconomicActor economicActorSelf => this;

        public OrdersSender(LocalizedString name, Market market)
        {
            _name = name;
            this.market = market;
        }
        
        public OrdersSender SendOrders(List<OrderSummary> orderSummaries)
        {
            foreach (var order in orderSummaries)
            {
                economicActorSelf.SetOrder(order);
            }
            
            return this;
        }

        public OrdersSender SendBackOrders()
        {
            economicActorSelf.RemoveAllOrders();
            return this;
        }
    }
}