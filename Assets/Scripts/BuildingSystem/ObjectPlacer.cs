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
            if (!_currentPlaceableObject && Input.GetKeyUp(KeyCode.B))
            {
                Instantiate(_placeableObjectTest.gameObject).TryGetComponent(out _currentPlaceableObject);
            }

            if (_currentPlaceableObject)
            {
                GridManager.ResetTileMap();
                
                SnapObjectToGrid();
                
                if (Input.GetKeyUp(KeyCode.R))
                {
                    _currentPlaceableObject.Rotate();    
                }

                if (GridManager.ObjectIsPlaceable(GridManager.HoveredCell, _currentPlaceableObject))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        PlaceObject();   
                    }
                    else
                    {
                        GridManager.DisplayFeedBackTiles();
                    }
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (BuildingHovering.HoveredBuilding)
                {
                    BuildingHovering.HoveredBuilding.Destroy();
                    if(_currentPlaceableObject)
                        GridManager.ResetTileMap();
                }
            }
        }

        private void SnapObjectToGrid()
        {
            _currentPlaceableObject.transform.position = GridManager.Instance.cursorPosition;
        }

        private void PlaceObject()
        {
            _currentPlaceableObject.Place(GridManager.HoveredCell);
            _currentPlaceableObject = null;
        }
    }
}