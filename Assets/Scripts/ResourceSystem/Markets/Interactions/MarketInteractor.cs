using System;
using GridSystem;
using GridSystem.Interaction;
using ResourceSystem.Markets.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace ResourceSystem.Markets.Interactions
{
    
    public class MarketInteractor : Singleton<MarketInteractor>, IGridInteractor
    {
        public GridInteractorType type => GridInteractorType.Market;
        public bool isActive { get; set; }
        public bool cancelable => false;

        public void OnHoveredCellChanged(CellData cellData)
        {
            if (!isActive) return;
            
            Market hoveredMarket = cellData?.market;

            if (s_hoveredMarket != null && s_hoveredMarket != s_selectedMarket)
            {
                if (hoveredMarket == null || hoveredMarket != s_hoveredMarket)
                    UnHighlightMarket(s_hoveredMarket);
            }

            s_hoveredMarket = hoveredMarket;

            HighlightMarket(s_hoveredMarket);
        }

        void IGridInteractor.OnActivated()
        {
            
        }

        void IGridInteractor.OnDeactivated()
        {
            UnSelectMarket();
            s_hoveredMarket = null;
        }

        private static Market s_selectedMarket;
        private static Market s_hoveredMarket;
        public static Market SelectedMarket => s_selectedMarket;
        
        public TileBase highlightMarketTile;

        private void Awake()
        {
            MarketInfoPanel.Instance.onClose.AddListener(UnSelectMarket);
        }

        private void Update()
        {
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
            MarketInfoPanel.Instance.ClosePanel();
        }

        private static void HighlightMarket(Market market)
        {
            if (market == null) return;

            GridManager.PaintTilemap(MarketManager.Instance.marketTile, TileMapType.Market,
                market.color + Color.white * 0.2f, market.cells.ToArray());
        }

        private static void UnHighlightMarket(Market market)
        {
            if (market == null) return;
            
            GridManager.PaintTilemap(MarketManager.Instance.marketTile, TileMapType.Market,
                market.color, market.cells.ToArray());
        }
    }
}