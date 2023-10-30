using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridSystem;
using ResourceSystem.Markets;

namespace BuildingSystem.Facilities.FacilityTypes
{
    public class DepositFacility : FacilityType
    {
        public int marketMergeRange = 1;
        private static readonly List<DepositFacility> s_deposits = new();
        private CellData[] _influencedArea;

        private List<CellData> GetExclusiveMarketCells()
        {
            List<CellData> exclusiveCells = new List<CellData>(_influencedArea);

            foreach (var cellData in _influencedArea)
            {
                Parallel.ForEach(s_deposits, deposit =>
                {
                    if(deposit == this) return;
                    if (!Array.Exists(deposit._influencedArea, _cellData => _cellData == cellData)) return;
                    if (exclusiveCells.Contains(cellData))
                        exclusiveCells.Remove(cellData);
                });
            }
            
            return exclusiveCells;
        }
        
        public override void OnAddedToCell(CellData cellAddedTo)
        {
            base.OnAddedToCell(cellAddedTo);
            s_deposits.Add(this);
            _influencedArea = GridManager.GetNeighbours(cellAddedTo, marketMergeRange, true);

            Market marketToMergeInto = null;
            
            foreach (var cellData in _influencedArea)
            {
                if (cellData.market.isEcosystem) continue;
                marketToMergeInto = cellData.market;
                break;
            }

            if (marketToMergeInto == null) marketToMergeInto = MarketManager.AddMarket(false,  cellAddedTo);
            
            foreach (var cellData in _influencedArea)
            {
                if (marketToMergeInto == cellData.market) continue; 
                MarketManager.MergeMarkets(marketToMergeInto, cellData.market, out _);
            }
        }

        public override void OnRemovedFromCell(CellData cellRemovedFrom)
        {
            MarketManager.AmputateMarket(cell.market, GetExclusiveMarketCells());
            base.OnRemovedFromCell(cellRemovedFrom);
            s_deposits.Remove(this);
        }
    }
}