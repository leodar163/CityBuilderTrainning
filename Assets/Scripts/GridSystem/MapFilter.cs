using System;
using GridSystem.Interaction;
using Utils;

namespace GridSystem
{
    public class MapFilter : Singleton<MapFilter>
    {
        private TileMapType _currentFilter = TileMapType.Terrain;

        public static TileMapType CurrentFilter => Instance._currentFilter;

        public static void ShowMapFilter(TileMapType tileMapType)
        {
            if (tileMapType == Instance._currentFilter) return;
            
            if (Instance._currentFilter != TileMapType.Terrain)
                GridManager.ShowTileMap(Instance._currentFilter, false);

            Instance._currentFilter = tileMapType;
            if (Instance._currentFilter != TileMapType.Terrain)
                GridManager.ShowTileMap(Instance._currentFilter, true);

            switch (tileMapType)
            {
                case TileMapType.Feedback:
                    break;
                case TileMapType.Market:
                    GridEventSystem.SwitchDefaultInteractor(GridInteractorType.Market);
                    break;
                case TileMapType.Terrain:
                    GridEventSystem.SwitchDefaultInteractor(GridInteractorType.Terrain);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tileMapType), tileMapType, null);
            }
        }
    }
}