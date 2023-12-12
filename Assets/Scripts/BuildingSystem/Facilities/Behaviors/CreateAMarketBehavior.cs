using System;
using System.Collections.Generic;
using GridSystem;
using ResourceSystem.Markets;
using UnityEngine;

namespace BuildingSystem.Facilities.Behaviors
{
    [CreateAssetMenu(menuName = "Facilities/Behaviors/Create a Market", fileName = "PlaceAMarketBehavior")]
    public class CreateAMarketBehavior : FacilityBehavior
    {
        private static readonly Dictionary<CellData, List<FacilityType>> s_influenceAreas = new();
        private static readonly Dictionary<FacilityType, CellData[]> s_zonesByBehavior = new();

        [SerializeField] [Min(0)] private int marketRange = 1; 
        
        public override void OnAddedToCell(FacilityType facility, CellData cell)
        {
            CellData[] area = GridManager.GetNeighbours(cell, marketRange, true);

            s_zonesByBehavior.Add(facility, area);
            
            Market market = null;

            foreach (var neighbour in area)
            {
                if (neighbour.market is { type: MarketType.Artificial })
                {
                    market = neighbour.market;
                    break;
                }
            }

            if (market == null)
            {
                market = MarketManager.AddMarket(MarketType.Artificial, area);
            }
            
            foreach (var neighbour in area)
            {
                if (!s_influenceAreas.TryAdd(neighbour, new List<FacilityType> { facility }))
                {
                    s_influenceAreas[neighbour].Add(facility);
                }
                
                if (neighbour.market == null)
                {
                    throw new NullReferenceException($"cell {neighbour.coordinates} doesn't have market while trying to merge it");
                }
                else switch (neighbour.market.type)
                {
                    case MarketType.Artificial:
                        if (neighbour.market != market) MarketManager.MergeMarkets(neighbour.market,market, out _);
                        break;
                    case MarketType.Ecosystem:
                        MarketManager.TransferCells(neighbour.market, market, neighbour);
                        break;
                    case MarketType.Both:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            foreach (var border in market.outerBorder)
            {
                if (border.market != market && border.market.type == MarketType.Artificial)
                {
                    MarketManager.MergeMarkets(border.market, market, out _);
                }
            }
        }

        public override void OnRemovedFromCell(FacilityType facility, CellData cell)
        {
            List<CellData> cellsToAmputate = new List<CellData>();

            foreach (var neighbour in s_zonesByBehavior[facility])
            {
                s_influenceAreas[neighbour].Remove(facility);
                if (s_influenceAreas[neighbour].Count == 0)
                {
                    s_influenceAreas.Remove(neighbour);
                    cellsToAmputate.Add(neighbour);
                }
            }
            
            MarketManager.AmputateMarket(cell.market, cellsToAmputate);
            s_zonesByBehavior.Remove(facility);
        }
    }
}