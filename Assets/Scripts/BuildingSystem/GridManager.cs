using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace BuildingSystem
{
    public class GridManager : Singleton<GridManager>
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private Tilemap _tilemap;

        [SerializeField] private Transform _cursor;
        [SerializeField] private Tile _rangeTile;

        private Camera _mainCamera;

        private List<Vector3Int> _blockedCells = new List<Vector3Int>();
        private Vector3Int _hoveredCell;

        public Vector3 cursorPosition => _cursor.position;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (TryGetHoveredTile(out _hoveredCell))
            {
                _cursor.gameObject.SetActive(true);
                 Vector3 newCursorPosition = _grid.GetCellCenterWorld(_hoveredCell);
                 newCursorPosition.y = 0.0001f;
                 _cursor.position = newCursorPosition;
            }
            else
            {
                _cursor.gameObject.SetActive(false);
            }
        }

        private bool TryGetHoveredTile(out Vector3Int hoveredCell)
        {
            Ray mouseRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit hit, 100, LayerMask.GetMask("Terrain")))
            {
                hoveredCell = _grid.WorldToCell(hit.point);
                return true; 
            }

            hoveredCell = default;
            return false;
        }

        private void PaintTileMap(params Vector3Int[] cellsToPaint)
        {
            
        }

        public bool CheckIfObjectIfPlaceable(Vector3Int referentialCell, PlaceableObject placeableObject)
        {
            foreach (var cellOffset in placeableObject.range)
            {
                if (CheckIfCellIsBlocked(cellOffset + referentialCell)) return true;
            }

            return false;
        }
        
        public bool CheckIfCellIsBlocked(Vector3Int cell)
        {
            if (_blockedCells.Contains(cell)) return true;
            return false;
        }
        
        public bool TryPlaceObject(PlaceableObject placeableObject)
        {
            if (_hoveredCell == default || CheckIfObjectIfPlaceable(_hoveredCell, placeableObject)) return false;

            placeableObject.Place(_hoveredCell);
            
            foreach (var cellOffset in placeableObject.range)
            {
                _blockedCells.Add(_hoveredCell + cellOffset);
            }

            return true;
        }
    }
}