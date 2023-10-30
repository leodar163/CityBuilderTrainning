using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridSystem;
using ResourceSystem.Markets.PeopleManagement;
using UnityEngine;

namespace ResourceSystem.Markets
{
    public class Market
    {
        public string name = "market_no_name";
        public bool isEcosystem = true;
        public readonly List<CellData> cells = new();
        private CellData[] _innerBorder;
        private CellData[] _outerBorder;

        public Color color = Color.magenta;
        
        public CellData[] innerBorder => _innerBorder;
        public CellData[] outerBorder => _outerBorder;

        private readonly List<ResourceOrder> _orders = new();
        public readonly List<ResourceValue> _resourceValues = new();

        public PeopleNeedManager _peopleNeedManager;

        #region CONSTRUCTORS

        public Market(bool isEcosystem)
        {
            this.isEcosystem = isEcosystem;

            if (!isEcosystem)
                _peopleNeedManager = new PeopleNeedManager(this);
        }
        
        public Market(Color color, bool isEcosystem) : this(isEcosystem)
        {
            this.color = color;
        }

        #endregion
        
        public void AddCell(CellData cell)
        {
            if (cells.Contains(cell)) return;
            
            cells.Add(cell);
            cell.market = this;
        }

        public void RemoveCell(CellData cell)
        {
            if (!cells.Contains(cell)) return;
            
            cells.Remove(cell);

            if (cell.market == this) cell.market = MarketManager.AddMarket(cell, 0);

            Parallel.ForEach(cell.economicActors, actor =>
            {
                actor.NotifyMarketChange();
            });
        }

        public void CalculateBorders()
        {
            _innerBorder = GridManager.GetInnerBorderOfArea(cells);
            _outerBorder = GridManager.GetOuterBorderOfArea(cells);
        }

        public void NotifyOrderChange(ResourceOrder order)
        {
            if (order.market != this) return;

            if (!_orders.Contains(order))
            {
                _orders.Add(order);
                
            }
            else if (order.quantity == 0)
            {
                _orders.Remove(order);
            }

            if (!TryGetResourceValue(order.resource, out ResourceValue resourceValue))
            {
                resourceValue = new ResourceValue(order.resource);
                _resourceValues.Add(resourceValue);
            }

            CalculateResourceValueAmount(resourceValue, order.type);

            if (resourceValue.demand == 0 && resourceValue.offer == 0)
            {
                _resourceValues.Remove(resourceValue);
            }
            else
            {
                CalculateResourceValueAvailability(resourceValue);
            }
        }

        private void CalculateResourceValueAmount(ResourceValue resourceValue, OrderType orderType)
        {
            switch (orderType)
            {
                case OrderType.Demand:
                    resourceValue.demand = 0;
                    break;
                case OrderType.Offer:
                    resourceValue.offer = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null);
            }
            
            Parallel.ForEach(_orders, order =>
            {
                if (order.resource != resourceValue.resource || order.type != orderType) return;
                
                switch(orderType)
                {
                    case OrderType.Demand:
                        resourceValue.demand += order.quantity;
                        break;
                    case OrderType.Offer:
                        resourceValue.offer += order.quantity;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null);
                }
            });
        }

        private static void CalculateResourceValueAvailability(ResourceValue resourceValue)
        {
            resourceValue.availability =
                resourceValue.offer == 0 ? 0 :
                resourceValue.demand == 0 ? 1 :
                    Mathf.Clamp01(resourceValue.offer / resourceValue.demand);
        }

        public float GetResourceValueAmount(ResourceType resource, OrderType orderType)
        {
            if (TryGetResourceValue(resource, out ResourceValue resourceValue))
            {
                return orderType switch
                {
                    OrderType.Demand => resourceValue.demand,
                    OrderType.Offer => resourceValue.offer,
                    _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null)
                };
            }

            return -1;
        }
        
        public float GetResourceAvailability(ResourceType resource)
        {
            if (TryGetResourceValue(resource, out ResourceValue resourceValue))
            {
                return resourceValue.availability;
            }

            return 0;
        }

        public bool TryGetResourceValue(ResourceType resource, out ResourceValue resourceValue)
        {
            foreach (var value in _resourceValues)
            {
                if (value.resource == resource)
                {
                    resourceValue = value;
                    return true;
                }
            }

            resourceValue = null;
            return false;
        }

        public void OnRemoved()
        {
            if (_peopleNeedManager == null) return;
            
            _peopleNeedManager.economicActorSelf.RemoveAllOrders();
            _peopleNeedManager.market = null;
            _peopleNeedManager.OnRemoved();
            _peopleNeedManager = null;
        }
    }
}