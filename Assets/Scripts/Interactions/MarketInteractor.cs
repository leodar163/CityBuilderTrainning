using System;
using GridSystem;
using ResourceSystem.Market;
using ResourceSystem.Market.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Interactions
{
    public class MarketInteractor : Singleton<MarketInteractor>, IInteractor
    {
        public bool isActive { get; private set; }
        public InteractionMode interactionMode => InteractionMode.MarketVue;
        
        private static Market s_selectedMarket;
        private static Market s_hoveredMarket;
        public static Market SelectedMarket => s_selectedMarket;
        
        public TileBase highlightMarketTile;

        private void Awake()
        {
            DeactivateMode();
            MarketInfoPanel.Instance.onClose.AddListener(UnSelectMarket);
        }

        private void Update()
        {
            if (!isActive) return;

            Market hoveredMarket = GridManager.HoveredCell?.market;

            if (s_hoveredMarket != null && s_hoveredMarket != s_selectedMarket)
            {
                if (hoveredMarket == null || hoveredMarket != s_hoveredMarket)
                    UnHighlightMarket(s_hoveredMarket);
            }

            s_hoveredMarket = hoveredMarket;

            HighlightMarket(s_hoveredMarket);

            if (s_hoveredMarket != null && Input.GetMouseButtonUp(0))
            {
                SelectMarket(s_hoveredMarket);
            }
        }

        public static void SelectMarket(Market market)
        {
            if (market == null) return;
            
            if (s_selectedMarket != null && market != s_selectedMarket)
            {
                UnHighlightMarket(s_selectedMarket);    
            }
            
            s_selectedMarket = market;

            HighlightMarket(s_selectedMarket);

            MarketInfoPanel.Instance.OpenPanel();
        }

        public static void UnSelectMarket()
        {
            if (s_selectedMarket == null) 
                return;

            UnHighlightMarket(s_selectedMarket);
            
            s_selectedMarket = null;
        }
        
        public static void HighlightMarket(Market market)
        {
            if (market == null) return;

            GridManager.PaintTilemap(MarketManager.Instance.marketTile, GridManager.TileMapType.Market,
                market.color + Color.white * 0.2f, market.cells.ToArray());
        }

        public static void UnHighlightMarket(Market market)
        {
            if (market == null) return;
            
            GridManager.PaintTilemap(MarketManager.Instance.marketTile, GridManager.TileMapType.Market,
                market.color, market.cells.ToArray());
        }
        
        public void ActivateMode()
        {
            isActive = true;
            GridManager.ShowTileMap(GridManager.TileMapType.Market, isActive);
        }

        public void DeactivateMode()
        {
            isActive = false;

            s_hoveredMarket = null;
            
            UnSelectMarket();
            
            GridManager.ShowTileMap(GridManager.TileMapType.Market, isActive);
        }
    }
}