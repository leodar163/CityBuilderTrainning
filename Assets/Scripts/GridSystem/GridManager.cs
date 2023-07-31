using System;
using System.Collections.Generic;
using BuildingSystem;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace GridSystem
{
    public class GridManager : Singleton<GridManager>
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private Tilemap _feedBackTileMap;

        [SerializeField] private BoundsInt _size = new BoundsInt(-62, -62, 0, 124, 124, 0);
        
        [SerializeField] private Transform _cursor;
        [Header("Tiles")]

        private Camera _mainCamera;

        private CellData[,] _cellDatas; 
        private Vector3Int _hoveredCell;

        public Vector3 cursorPosition => _cursor.position;
        public static Vector3Int HoveredCell => Instance._hoveredCell;

        public enum TileMapType
        {
            Feedback
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
            InitCellData();
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

        private void InitCellData()
        {
            _cellDatas = new CellData[_size.size.x, _size.size.y];
           
            for (int i = 0; i < _cellDatas.GetLength(0); i++)
            {
                for (int j = 0; j < _cellDatas.GetLength(1); j++)
                {
                    _cellDatas[i, j] = new CellData
                    {
                        cell = new Vector3Int(i, j)
                    };
                }
            }
        }
        
        private bool TryGetHoveredTile(out Vector3Int hoveredCell)
        {
            Ray mouseRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit hit, 100, LayerMask.GetMask("Terrain")))
            {
                hoveredCell = _grid.WorldToCell(hit.point);
                hoveredCell.x = Mathf.Clamp(HoveredCell.x, Instance._size.xMin, Instance._size.xMax);
                hoveredCell.y = Mathf.Clamp(HoveredCell.y, Instance._size.yMin, Instance._size.yMax);
                return true; 
            }

            hoveredCell = default;
            return false;
        }

        public static void ResetTileMap(TileMapType tileMapType)
        {
            Tilemap resetedMap = GetTilemap(tileMapType);

            Vector3Int cursor = Vector3Int.zero;

            for (int y = Instance._size.yMin; y < Instance._size.yMax; y++)
            {
                for (int x = Instance._size.xMin; x < Instance._size.xMax; x++)
                {
                    cursor.x = x;
                    cursor.y = y;
                    resetedMap.SetTile(cursor, null);
                }   
            }
        }

        private static Tilemap GetTilemap(TileMapType tileMapType)
        {
            return tileMapType switch 
            {
                TileMapType.Feedback => Instance._feedBackTileMap,
                _ => throw new ArgumentOutOfRangeException(nameof(tileMapType), tileMapType, null)
            };
        }
        
        public static void PaintTilemap(Tile paintTile, TileMapType tilemap, params Vector3Int[] cellsToPaint)
        {
            Tilemap mapToPaint = GetTilemap(tilemap);
            foreach (var cell in cellsToPaint)
            {
                if(CellIsOutOfGrid(cell)) break;
                mapToPaint.SetTile(cell, paintTile);
            }
        }
        
        public static bool ObjectIsPlaceable(Vector3Int referentialCell, PlaceableObject placeableObject)
        {
            foreach (var cell in placeableObject.GetAbsoluteCellRange(referentialCell))
            {
                if (CellIsBlocked(cell) || CellIsOutOfGrid(cell))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ObjectIsPlaceable(Vector3Int referentialCell, PlaceableObject placeableObject,
            out Vector3Int[] blockedCells, out Vector3Int[] freeCells)
        {
            bool isBlocked = false;
            List<Vector3Int> blockedTiles = new List<Vector3Int>();
            List<Vector3Int> freeTiles = new List<Vector3Int>();
            
            foreach (var cell in placeableObject.GetAbsoluteCellRange(referentialCell))
            {
                if (CellIsBlocked(cell))
                {
                    isBlocked = true;
                    blockedTiles.Add(cell);
                }
                else if (CellIsOutOfGrid(cell))
                {
                    isBlocked = true;
                }
                else
                {
                    freeTiles.Add(cell);
                }
            }

            blockedCells = blockedTiles.ToArray();
            freeCells = freeTiles.ToArray();
            return !isBlocked;
        }

        public static bool CellIsOutOfGrid(Vector3Int cell)
        {
            return cell.x < Instance._size.xMin || cell.y < Instance._size.yMin ||
                    cell.x > Instance._size.xMax || cell.y > Instance._size.yMax;
        }
        
        public static bool CellIsBlocked(Vector3Int cell)
        {
            return GetCellData(cell).isBlocked;
        }

        public static ref CellData GetCellData(Vector3Int cell)
        {
            return ref Instance._cellDatas[cell.x - Instance._size.xMin, cell.y - Instance._size.yMin];
        }

    }
}