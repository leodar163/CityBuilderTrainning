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
            if (!_currentPlaceableObject)
            {
                if (Input.GetMouseButtonUp(1))
                {
                    Instantiate(_placeableObjectTest.gameObject).TryGetComponent(out _currentPlaceableObject);
                }
                return;
            }
            
            _currentPlaceableObject.transform.position = GridManager.Instance.cursorPosition;

            if (Input.GetMouseButtonUp(0) && GridManager.Instance.TryPlaceObject(_currentPlaceableObject))
            {
                _currentPlaceableObject = null;
            }
        }
    }
}