using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridSystem;
using ResourceSystem.Markets;

namespace BuildingSystem.Facilities.FacilityTypes
{
    [Serializable]
    public class MarketCreatorFacility : FacilityType
    {
        public int marketMergeRange = 1;
        private static readonly List<MarketCreatorFacility> s_deposits = new();
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
                if (cellData.market.type == MarketType.Ecosystem) continue;
                marketToMergeInto = cellData.market;
                break;
            }

            if (marketToMergeInto == null) marketToMergeInto = MarketManager.AddMarket(MarketType.Artificial,  cellAddedTo);
            
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

        public override FacilityType Copy()
        {
            return new MarketCreatorFacility
            {
                _renderData = _renderData,
                _scaleMultiplier = _scaleMultiplier,
                _facilityName = _facilityName,
                _facilityDescription = _facilityDescription,
                _placementCondition = _placementCondition,
                constructionCost = constructionCost,
                _sizeRadius = _sizeRadius,
                
                marketMergeRange = marketMergeRange
            };
        }
    }
}