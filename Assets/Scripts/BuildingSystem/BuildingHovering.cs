using System;
using UnityEngine;
using Utils;

namespace BuildingSystem
{
    public class BuildingHovering : Singleton<BuildingHovering>
    {
        private PlaceableObject _currentHoveredBuilding;
        public static PlaceableObject HoveredBuilding => Instance._currentHoveredBuilding;

        private Camera _mainCamera;
        
        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            TryGetHoveredBuilding(out _currentHoveredBuilding);
        }

        private bool TryGetHoveredBuilding(out PlaceableObject hoveredBuilding)
        {
            Ray mouseRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(mouseRay, out RaycastHit hit, 100, LayerMask.GetMask("Buildings")))
            {
                hit.collider.TryGetComponent(out hoveredBuilding);
            }
            else
            {
                hoveredBuilding = null;
            }
            return hoveredBuilding;
        }
    }
}