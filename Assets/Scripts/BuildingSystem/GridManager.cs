using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Utils;

namespace BuildingSystem
{
    public class GridManager : Singleton<GridManager>
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private Tilemap _feedBackTileMap;

        [SerializeField] private Transform _cursor;
        [Header("Tiles")]
        [SerializeField] private Tile _rangeTile;
        [SerializeField] private Tile _blockTile;

        private Camera _mainCamera;

        private readonly List<Vector3Int> _blockedCells = new ();
        private Vector3Int _hoveredCell;

        public Vector3 cursorPosition => _cursor.position;
        public static Vector3Int HoveredCell => Instance._hoveredCell;


        private readonly List<Vector3Int> _feedBackBlockedTiles = new List<Vector3Int>();
        private readonly List<Vector3Int> _feedBackRangeTiles = new List<Vector3Int>();


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

        public static void DisplayFeedBackTiles()
        {
            foreach (var cell in Instance._feedBackBlockedTiles)
            {
                Instance._feedBackTileMap.SetTile(cell, Instance._blockTile);
            }

            foreach (var cell in Instance._feedBackRangeTiles)
            {
                Instance._feedBackTileMap.SetTile(cell, Instance._rangeTile);
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

        public static void ResetTileMap()
        {
            foreach (var cell in Instance._feedBackBlockedTiles)
            {
                Instance._feedBackTileMap.SetTile(cell, null);
            }

            foreach (var cell in Instance._feedBackRangeTiles)
            {
                Instance._feedBackTileMap.SetTile(cell, null);
            }
            
            Instance._feedBackBlockedTiles.Clear();
            Instance._feedBackRangeTiles.Clear();
        }
        
        
        
        public static bool ObjectIsPlaceable(Vector3Int referentialCell, PlaceableObject placeableObject)
        {
            bool isBlocked = false;
            foreach (var cell in placeableObject.GetAbsoluteCellRange(referentialCell))
            {
                if (CheckIfCellIsBlocked(cell))
                {
                    isBlocked = true;
                    Instance._feedBackBlockedTiles.Add(cell);
                }
                else
                {
                    Instance._feedBackRangeTiles.Add(cell);
                }
            }

            return !isBlocked;
        }

        public static bool CheckIfCellIsBlocked(Vector3Int cell)
        {
            if (Instance._blockedCells.Contains(cell)) return true;
            return false;
        }
        
        public static void BlockCells(params Vector3Int[] cellsToBlock)
        {
            foreach (var cell in cellsToBlock)
            {
                if (CheckIfCellIsBlocked(cell))
                {
                    Debug.LogWarning($"Try to block cell({cell}) while it's already free");
                }
                else
                {
                    Instance._blockedCells.Add(cell);
                }   
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void FreeCells(params Vector3Int[] cellsToFree)
        {
            foreach (var cell in cellsToFree)
            {
                if (!Instance._blockedCells.Contains(cell))
                {
                    Debug.LogWarning($"Try to free cell({cell}) while it's already free");
                }
                else
                {
                    Instance._blockedCells.Remove(cell);
                }
            }
        }
    }
}