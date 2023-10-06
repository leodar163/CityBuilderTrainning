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
    }
}