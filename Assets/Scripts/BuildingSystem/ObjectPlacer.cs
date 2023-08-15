using GridSystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Utils;

namespace BuildingSystem
{
    public class ObjectPlacer : Singleton<ObjectPlacer>
    {
        private PlaceableObject _currentPlaceableObject;

        [SerializeField] private PlaceableObject _placeableObjectTest;
        
        [SerializeField] private Tile _freeTile;
        [SerializeField] private Tile _blockTile;

        private void Update()
        {
            if (!_currentPlaceableObject && Input.GetKeyUp(KeyCode.B))
            {
                Instantiate(_placeableObjectTest.gameObject).TryGetComponent(out _currentPlaceableObject);
            }

            if (_currentPlaceableObject)
            {
                GridManager.ResetTileMap(GridManager.TileMapType.Feedback);
                
                SnapObjectToGrid();
                
                if (Input.GetKeyUp(KeyCode.R))
                {
                    _currentPlaceableObject.Rotate();    
                }

                if (GridManager.ObjectIsPlaceable(GridManager.HoveredCell.cellCoordinates, _currentPlaceableObject,
                        out Vector3Int[] blockedCells, out Vector3Int[] freeCells))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        PlaceObject();   
                        GridManager.PaintTilemap(null, GridManager.TileMapType.Feedback, blockedCells);
                        GridManager.PaintTilemap(null, GridManager.TileMapType.Feedback, freeCells);
                        return;
                    }
                }
                
                GridManager.PaintTilemap(_blockTile, GridManager.TileMapType.Feedback, blockedCells);
                GridManager.PaintTilemap(_freeTile, GridManager.TileMapType.Feedback, freeCells);
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (BuildingHovering.HoveredBuilding)
                {
                    BuildingHovering.HoveredBuilding.Destroy();
                    if(_currentPlaceableObject)
                        GridManager.ResetTileMap(GridManager.TileMapType.Feedback);
                }
            }
        }

        private void SnapObjectToGrid()
        {
            _currentPlaceableObject.transform.position = GridManager.Instance.cursorPosition;
        }

        private void PlaceObject()
        {
            _currentPlaceableObject.Place(GridManager.HoveredCell.cellCoordinates);
            _currentPlaceableObject = null;
        }
    }
}