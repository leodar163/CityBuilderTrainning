using System.Collections.Generic;
using GridSystem;
using UnityEngine;

namespace ResourceSystem.Markets
{
    public interface IEconomicActor
    {
        public Market market { get; }
        
        protected List<ResourceOrder> orders { get; }

        public IEconomicActor economicActorSelf { get; }

        public void RemoveAllOrders()
        {
            foreach (var order in orders.ToArray())
            {
                RemoveOrder(order);
            }
        }
        
        public void RemoveOrder(ResourceType resource, OrderType orderType)
        {
            SetOrder(resource, 0, orderType);
        }

        public void RemoveOrder(ResourceOrder orderToRemove)
        {
            SetOrder(orderToRemove, 0);
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
            quantity = Mathf.Clamp(quantity, 0, float.PositiveInfinity);
            
            if (TryGetOrder(resource, orderType, out ResourceOrder order))
            {
                SetOrder(order, quantity);
                return;
            }

            if (quantity == 0) return;

            order = new ResourceOrder(resource, quantity, market, this, orderType);
            
            orders.Add(order);
            market.NotifyOrderChange(order);
        }

        public void SetOrder(ResourceOrder order, float quantity)
        {
            if (!orders.Contains(order)) return;
            
            if (quantity == 0)
                orders.Remove(order);
                
            order.quantity = quantity;
            order.market.NotifyOrderChange(order);
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