﻿using System.Collections.Generic;
using GridSystem;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;
using Random = UnityEngine.Random;

namespace ResourceSystem.Markets
{
    public class MarketManager : Singleton<MarketManager>
    {
        public TileBase marketTile;

        public static readonly List<Markets.Market> markets = new();

        [SerializeField] private int _maxDistanceToMerge = 5;

        public static int MaxDistanceToMerge
        {
            get => Instance._maxDistanceToMerge;
            set => Instance._maxDistanceToMerge = value;
        }

        public enum MergeCase
        {
            noError,
            tooFarAway,
            overlapping,
            adjacent
        }

        public static Markets.Market AddMarket(CellData originCell, int range, bool isEcosystem = true)
        {
            return AddMarket(isEcosystem, GridManager.GetNeighbours(originCell, range, true));
        }

        public static Markets.Market AddMarket(bool isEcosystem = true, params CellData[] area )
        {
            Markets.Market market = new()
            {
                color = Random.ColorHSV(),
                isEcosystem = isEcosystem
            };

            foreach (var cell in area)
            {
                Markets.Market oldMarket = cell.market;

                market.AddCell(cell);
                cell.market = market;
                
                oldMarket?.RemoveCell(cell);
                if (oldMarket != null && oldMarket.cells.Count == 0) RemoveMarket(oldMarket); 
            }
            
            GridManager.PaintTilemap(Instance.marketTile, GridManager.TileMapType.Market, market.color, area);

            markets.Add(market);
            
            market.CalculateBorders();
            
            return market;
        }

        public static float GetDistanceBetweenMarkets(Markets.Market a, Markets.Market b)
        {
            return GridManager.GetMinDistanceBetweenAreas(a.innerBorder, b.innerBorder);
        }

        public static bool AreMarketOverlapping(Markets.Market a, Markets.Market b)
        {
            foreach (var cell in a.cells)
            {
                if (b.cells.Contains(cell)) return true;
            }

            return false;
        }

        public static bool AreMarketAdjacent(Markets.Market a, Markets.Market b)
        {
            foreach (var border in a.outerBorder)
            {
                if (b.cells.Contains(border)) return true;
            }

            return false;
        }
        
        public static bool CanMergeMarkets(Markets.Market origin, Markets.Market target, out MergeCase mergeCase)
        {
            if (AreMarketOverlapping(origin, target))
            {
                mergeCase = MergeCase.overlapping;
                return false;
            }

            if (AreMarketAdjacent(origin, target))
            {
                mergeCase = MergeCase.adjacent;
                return true;
            }
            
            float distance = GetDistanceBetweenMarkets(origin, target);

            if (distance > Instance._maxDistanceToMerge)
            {
                mergeCase = MergeCase.tooFarAway;
                return false;
            }

            mergeCase = MergeCase.noError;
            return true;
        }

        public static void MergeMarkets(Markets.Market origin, Markets.Market target, out MergeCase mergeCase)
        {
            if (!CanMergeMarkets(origin, target, out mergeCase)) return;

            CellData[] targetArea = target.cells.ToArray();
            
            GridManager.PaintTilemap(Instance.marketTile, GridManager.TileMapType.Market, origin.color, targetArea);
            
            foreach (var cell in targetArea)
            {
                origin.AddCell(cell);
                target.RemoveCell(cell);
            }
            
            origin.CalculateBorders();
            RemoveMarket(target);
        }
        
        public static void RemoveMarket(Markets.Market marketToRemove)
        {
            if (!markets.Contains(marketToRemove)) return;

            foreach (var cell in marketToRemove.cells)
            {
                marketToRemove.RemoveCell(cell);
            }

            markets.Remove(marketToRemove);
        }

        public static void AmputateMarket(Markets.Market marketToAmputate, List<CellData> cellsToRemove)
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
                    AddMarket(false, area);
                }

                marketAreas.Remove(area);
            }
        }

        public static List<CellData[]> GetMarketsIsolatedAreas(Markets.Market market)
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