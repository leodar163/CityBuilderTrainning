using System.Collections.Generic;
using GridSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace ResourceSystem.Market
{
    public class MarketManager : Singleton<MarketManager>
    {
        public static readonly List<Market> markets = new();
        
        [SerializeField] private int _maxDistanceToMerge = 5;

        public static int MaxDistanceToMerge
        {
            get => Instance._maxDistanceToMerge;
            set => Instance._maxDistanceToMerge = value;
        }

        public enum MergeError
        {
            noError,
            tooFarAway,
            overlapping,
            adjacent
        }
        
        public static Market AddMarket(CellData originCell, int range)
        {
            return AddMarket(GridManager.GetNeighbours(originCell, range, true));
        }

        public static Market AddMarket(params CellData[] area)
        {
            Market market = new();

            foreach (var cell in area)
            {
                market.AddCell(cell);
            }

            markets.Add(market);
            
            market.CalculateBorders();
            
            return market;
        }
        
        public static float GetDistanceBetweenMarkets(Market a, Market b)
        {
            return GridManager.GetMinDistanceBetweenAreas(a.innerBorder, b.innerBorder);
        }

        public static bool AreMarketOverlapping(Market a, Market b)
        {
            foreach (var cell in a.cells)
            {
                if (b.cells.Contains(cell)) return true;
            }

            return false;
        }

        public static bool AreMarketAdjacent(Market a, Market b)
        {
            foreach (var border in a.outerBorder)
            {
                if (b.cells.Contains(border)) return true;
            }

            return false;
        }
        
        public static bool CanMergeMarkets(Market origin, Market target, out MergeError error)
        {
            if (AreMarketOverlapping(origin, target))
            {
                error = MergeError.overlapping;
                return false;
            }

            if (AreMarketAdjacent(origin, target))
            {
                error = MergeError.adjacent;
                return true;
            }
            
            float distance = GetDistanceBetweenMarkets(origin, target);

            if (distance > Instance._maxDistanceToMerge)
            {
                error = MergeError.tooFarAway;
                return false;
            }

            error = MergeError.noError;
            return true;
        }

        public static void MergeMarkets(Market origin, Market target, out MergeError error)
        {
            if (!CanMergeMarkets(origin, target, out error)) return;

            foreach (var cell in target.cells)
            {
                origin.AddCell(cell);
                target.RemoveCell(cell);
            }
            
            origin.CalculateBorders();
            RemoveMarket(target);
        }
        
        public static void RemoveMarket(Market marketToRemove)
        {
            if (!markets.Contains(marketToRemove)) return;

            foreach (var cell in marketToRemove.cells)
            {
                marketToRemove.RemoveCell(cell);
            }

            markets.Remove(marketToRemove);
        }

        public static void RemoveCellsFromMarket(Market marketToAmputate, List<CellData> cellsToRemove)
        {
            foreach (var cell in cellsToRemove)
            {
                marketToAmputate.RemoveCell(cell);
            }

            if (marketToAmputate.cells.Count == 0) RemoveMarket(marketToAmputate);
            
            marketToAmputate.CalculateBorders();

            //Now that market is amputated, it's possible that some areas are too much separated from each others
            //If it is the case, it will be needed to detect those area, then check on there distance from each others,
            //then, if the distance is too much, create a new market.
            
            List<CellData[]> marketAreas = GetMarketsIsolatedAreas(marketToAmputate);

            while (marketAreas.Count > 1)
            {
                CellData[] area = marketAreas[0];
                
                float distance = float.NegativeInfinity;
                
                foreach (var otherArea in marketAreas)
                {
                    float currentDistance = GridManager.GetMinDistanceBetweenAreas(area, otherArea);
                    if (currentDistance > distance) distance = currentDistance;
                }

                if (distance > Instance._maxDistanceToMerge)
                {
                    AddMarket(area);
                }

                marketAreas.Remove(area);
            }
        }

        public static List<CellData[]> GetMarketsIsolatedAreas(Market market)
        {
            List<CellData[]> areas = new();
            List<CellData> totalArea = new List<CellData>(market.cells);
            List<CellData> currentArea = new();
            Queue<CellData> openSet = new();

            while (totalArea.Count > 0)
            {
                if (openSet.Count == 0)
                {
                    if (currentArea.Count > 0)
                    {
                        areas.Add(currentArea.ToArray());
                        currentArea.Clear();
                    }
                    
                    openSet.Enqueue(totalArea[0]);
                    totalArea.RemoveAt(0);
                }
                
                CellData cell = openSet.Dequeue();

                foreach (var neighbour in cell.neighbours)
                {
                    if (!currentArea.Contains(neighbour) && market.cells.Contains(neighbour))
                    {
                        totalArea.Remove(neighbour);
                        currentArea.Add(neighbour);
                        openSet.Enqueue(neighbour);
                    }
                }
            }

            return areas;
        }
    }
}