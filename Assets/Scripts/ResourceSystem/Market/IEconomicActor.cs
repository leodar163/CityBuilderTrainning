using System.Collections.Generic;

namespace ResourceSystem.Market
{
    public interface IEconomicActor
    {
        public Market market { get; }
        
        protected List<ResourceOrder> orders { get; }

        public void RemoveOrder(ResourceType resource, OrderType orderType)
        {
            SetOrder(resource, 0, orderType);
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