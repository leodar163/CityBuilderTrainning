using System.Collections.Generic;
using GridSystem;
using UnityEngine;

namespace ResourceSystem.Market
{
    public class Market
    {
        public readonly List<CellData> cells = new();
        private CellData[] _innerBorder;
        private CellData[] _outerBorder;

        public Color color = Color.magenta;
        
        public CellData[] innerBorder => _innerBorder;
        public CellData[] outerBorder => _outerBorder;

        private List<ResourceOrder> orders = new();
        private List<(ResourceType, float, float)> demandOffers = new();

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
        }

        public void CalculateBorders()
        {
            _innerBorder = GridManager.GetInnerBorderOfArea(cells);
            _outerBorder = GridManager.GetOuterBorderOfArea(cells);
        }

        public void Destroy()
        {
                    
        }

        private void ReleaseAllOrders()
        {
            
        }
        
        public void NotifyOrderChange(ResourceOrder order)
        {
            if (order.market != this) return;

            if (!orders.Contains(order)) orders.Add(order);
            

            //Else, add in orders if not already, else, juste calculate new demand/offer ratio
        }

        private float GetOrderAmount(ResourceType resource)
        {
            
        }
        
        public float GetDemandOfferRatio(ResourceType resource)
        {
            foreach (var demandOffer in demandOffers)
            {
                if (demandOffer.Item1 == resource)
                {
                    return demandOffer.Item2 / demandOffer.Item3;
                }
            }

            return 0;
        }
    }
}