﻿using System.Collections.Generic;
using GridSystem;
using Localization;
using ResourceSystem.Markets.Interactions;
using ResourceSystem.Markets.Modifiers;
using ResourceSystem.Markets.Needs;
using TerrainSystem;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Tilemaps;
using Utils;
using Random = UnityEngine.Random;

namespace ResourceSystem.Markets
{
    public class MarketManager : Singleton<MarketManager>, IMarketModifierContainer
    {
        public TileBase marketTile;

        public static readonly List<Market> markets = new();
        public static readonly List<Market> artificialMarkets = new();
        public static readonly List<Market> ecosystemMarkets = new();

        public static readonly Market mainMarket = artificialMarkets.Count > 0 ? artificialMarkets[0] : null;
        
        [SerializeField] private Gradient _ecosystemColors;
        [SerializeField] private Gradient _humanMarketColors;
        [SerializeField] private int _maxDistanceToMerge = 1;
        [SerializeField] private ScriptableNeedsSet _needsSetTemplate;
        [Header("Formats")]
        [SerializeField] private LocalizedString _localizedEcosystemNameFormat;
        

        public IMarketModifierContainer modifierContainerSelf => this;
        public List<MarketModifier> modifiers { get; set; } = new();

        List<ResourceQuantity> IMarketModifierContainer.multipliers { get; } = new();
        public List<Market> Markets => markets;

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

        public static Market AddMarket(CellData originCell, int range, MarketType type = MarketType.Ecosystem)
        {
            return AddMarket(type, GridManager.GetNeighbours(originCell, range, true));
        }

        public static Market AddMarket(MarketType type = MarketType.Ecosystem, params CellData[] area)
        {
            Color marketColor = type == MarketType.Ecosystem
                ? GenerateMarketColor(area[0].terrain)
                : GenerateMarketColor(type);
            
            Market market = new(marketColor, type, Instance._needsSetTemplate.needsSet)
            {
                name = type == MarketType.Ecosystem
                    ? GenerateMarketName(area[0].terrain)
                    : GenerateMarketName(type)
            };

            foreach (var cell in area)
            {
                Market oldMarket = cell.market;

                market.AddCell(cell);
                cell.market = market;
                
                oldMarket?.RemoveCell(cell);
                if (oldMarket != null && oldMarket.cells.Count == 0) RemoveMarket(oldMarket);
            }
            
            GridManager.PaintTilemap(Instance.marketTile, TileMapType.Market, market.color, area);
            
            markets.Add(market);
            if (market.type is MarketType.Ecosystem or MarketType.Both)
            {
                ecosystemMarkets.Add(market);
            }

            if (market.type is MarketType.Artificial or MarketType.Both)
            {
                artificialMarkets.Add(market);
            }
                
            market.CalculateBorders();

            foreach (var mod in Instance.modifiers)
            {
                mod.ResetOrderSending();
            }
            
            return market;
        }

        private static Color GenerateMarketColor(MarketType marketType)
        {
            float alea = Random.Range(0, 1f);
            return marketType == MarketType.Ecosystem ? Instance._ecosystemColors.Evaluate(alea) : Instance._humanMarketColors.Evaluate(alea);
        }

        private static Color GenerateMarketColor(TerrainType terrain)
        {
            float alea = Random.Range(0, 1f);
            return terrain.ecosystemGradient.Evaluate(alea);
        }

        private static string GenerateMarketName(MarketType marketType)
        {
            return string.Format(Instance._localizedEcosystemNameFormat.GetLocalizedString(),
                (marketType == MarketType.Ecosystem
                    ? VariableNameManager.EcosystemName
                    : VariableNameManager.MarketName), "FOO");
        }

        private static string GenerateMarketName(TerrainType terrain)
        {
            return string.Format(Instance._localizedEcosystemNameFormat.GetLocalizedString(), terrain.terrainName, "FOO");
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
        
        public static bool CanMergeMarkets(Market origin, Market target, out MergeCase mergeCase)
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

        public static void MergeMarkets(Market origin, Market target, out MergeCase mergeCase)
        {
            if (!CanMergeMarkets(origin, target, out mergeCase)) return;

            CellData[] targetArea = target.cells.ToArray();
            
            GridManager.PaintTilemap(Instance.marketTile, TileMapType.Market, origin.color, targetArea);
            
            foreach (var cell in targetArea)
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

            if (marketToRemove == MarketInteractor.SelectedMarket)
            {
                MarketInteractor.UnSelectMarket();
            }
            
            marketToRemove.OnRemoved();
        }

        public static void AmputateMarket(Market marketToAmputate, params CellData[] cellsToRemove)
        {
            AmputateMarket(marketToAmputate, new List<CellData>(cellsToRemove));
        }
        
        public static void AmputateMarket(Market marketToAmputate, List<CellData> cellsToRemove)
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

            while (marketAreas is { Count: > 1 })
            {
                CellData[] area = marketAreas[0];
                marketAreas.Remove(area);
                
                float distance = float.NegativeInfinity;
                
                foreach (var otherArea in marketAreas)
                {
                    float currentDistance = GridManager.GetMinDistanceBetweenAreas(area, otherArea);
                    if (currentDistance > distance) distance = currentDistance;
                }

                if (distance > Instance._maxDistanceToMerge)
                {
                    AddMarket(marketToAmputate.type, area);
                }
            }
        }

        public static List<CellData[]> GetMarketsIsolatedAreas(Market market)
        {
            if (market.cells.Count == 0) return null;
            
            List<CellData[]> areas = new();
            List<CellData> totalArea = new List<CellData>(market.cells);
            List<CellData> currentArea = new();
            Queue<CellData> openSet = new();

            openSet.Enqueue(totalArea[0]);
            currentArea.Add(totalArea[0]);

            do
            {
                CellData cell = openSet.Dequeue();

                if (totalArea.Contains(cell)) totalArea.Remove(cell);

                foreach (var neighbour in cell.neighbours)
                {
                    if (!currentArea.Contains(neighbour) && market.cells.Contains(neighbour))
                    {
                        totalArea.Remove(neighbour);
                        currentArea.Add(neighbour);
                        openSet.Enqueue(neighbour);
                    }
                }
                
                if (openSet.Count == 0 || totalArea.Count == 0)
                {
                    if (currentArea.Count > 0)
                    {
                        areas.Add(currentArea.ToArray());
                        currentArea.Clear();
                    }

                    if (totalArea.Count > 0)
                    {
                        openSet.Enqueue(totalArea[0]);
                        currentArea.Add(totalArea[0]);
                    }
                }
                
            } while (totalArea.Count > 0);

            return areas;
        }

        public static void TransferCells(Market origin, Market target, params CellData[] cellsToTransfer)
        {
            foreach (var cell in cellsToTransfer)
            {
                target.AddCell(cell);
            }
            
            target.CalculateBorders();

            AmputateMarket(origin, cellsToTransfer);
        }
    }
}