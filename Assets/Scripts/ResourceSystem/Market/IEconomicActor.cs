using System.Collections.Generic;
using GridSystem;

namespace ResourceSystem.Market
{
    public interface IEconomicActor
    {
        public Market market => cell.market;
        
        public CellData cell { get; }
        
        protected List<ResourceOrder> orders { get; }

        public IEconomicActor economicActorSelf { get; }

        public void RemoveOrder(ResourceType resource, OrderType orderType)
        {
            SetOrder(resource, 0, orderType);
        }

        public void NotifyMarketChange()
        {
            foreach (var order in orders)
            {
                market.NotifyOrderChange(order);
            }
        }

        public void SetOrder(ResourceType resource, float quantity, OrderType orderType)
        {
            if (TryGetOrder(resource, orderType, out ResourceOrder order))
            {
                if (quantity == 0)
                    orders.Remove(order);
                
                order.quantity = quantity;
                order.market.NotifyOrderChange(order);
                return;
            }

            if (quantity == 0) return;
            
            orders.Add(new ResourceOrder(resource, quantity, market, this, orderType));
            market.NotifyOrderChange(order);
        }

        public bool TryGetOrder(ResourceType resource, OrderType orderType, out ResourceOrder order)
        {
            foreach (var currentOrder in orders)
            {
                if (currentOrder.resource != resource || currentOrder.type != orderType) continue;
                
                order = currentOrder;
                return true;
            }

            order = null;
            return false;
        }
    }
}