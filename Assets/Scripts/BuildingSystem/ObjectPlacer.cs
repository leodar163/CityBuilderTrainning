using System;
using UnityEngine;
using Utils;

namespace BuildingSystem
{
    public class ObjectPlacer : Singleton<ObjectPlacer>
    {
        private PlaceableObject _currentPlaceableObject = null;

        [SerializeField] private PlaceableObject _placeableObjectTest;
        
        private void Update()
        {
            ManageObjectPlacement();
        }

        private void ManageObjectPlacement()
        {
            GridManager.ResetTileMap();
            
            if (!_currentPlaceableObject)
            {
                if (Input.GetMouseButtonUp(1))
                {
                    Instantiate(_placeableObjectTest.gameObject).TryGetComponent(out _currentPlaceableObject);
                }
                return;
            }
            
            _currentPlaceableObject.transform.position = GridManager.Instance.cursorPosition;
            Vector3Int hoveredCell = GridManager.HoveredCell;
            
            
            if (GridManager.Instance.CheckIfObjectIfPlaceable(hoveredCell, _currentPlaceableObject)
            && Input.GetMouseButtonUp(0))
            {
                _currentPlaceableObject.Place(hoveredCell);
                _currentPlaceableObject = null;
            }
            
            GridManager.DisplayFeedBackTiles();
        }
    }
}