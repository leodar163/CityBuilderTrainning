using System;
using System.Collections.Generic;
using BuildingSystem;
using TimeSystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Utils;

namespace GridSystem
{
    [RequireComponent(typeof(BoxCollider))]
    public class GridManager : Singleton<GridManager>
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private BoxCollider _gridCollider;
        
        [Header("Tilemaps")]
        [SerializeField] private Tilemap _feedBackTileMap;
        [SerializeField] private Tilemap _terrainTileMap;
        
        [Header("Bounds")] 
        [SerializeField] private Vector2Int _gridSize = new Vector2Int(62, 62); 
        [SerializeField] private Vector2Int _positionOffset;
        
        [Space]
        [SerializeField] private Transform _cursor;
        private Material _cursorMat;

        private Camera _mainCamera;

        private CellData[,] _cellDatas; 
        private CellData _hoveredCell;

        private RectInt _gridRect;

        public static Vector2Int GridSize => Instance._gridSize; 
        public Vector3 cursorPosition => _cursor.position;
        public static CellData HoveredCell => Instance._hoveredCell;

        private bool _hoveringActivated = true;

        public static bool hoveringActivated { get => Instance._hoveringActivated; set => Instance._hoveringActivated = value;}

        public enum TileMapType
        {
            Feedback,
            Terrain
        }

        private void Awake()
        {
            TimeManager.onNewMonth += _ => MonthUpdateCells();
            
            if (_cursor.TryGetComponent(out MeshRenderer meshRend))
            {
                _cursorMat = meshRend.material;
            }
            
            _gridRect = new RectInt(-_gridSize/2 +_positionOffset, _gridSize);
            _mainCamera = Camera.main;
            InitCellData();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(new Vector3(_positionOffset.x + 0.5f, 0, _positionOffset.y + 0.5f),new Vector3(_gridSize.x, 0.01f, _gridSize.y));
        }

        private void OnValidate()
        {
            if (!_gridCollider) TryGetComponent(out _gridCollider);
            if (_gridCollider)
            {
                _gridCollider.size = new Vector3(_gridSize.x, 0.01f,_gridSize.y);
                _gridCollider.center = new Vector3(_positionOffset.x+ 0.5f, 0, _positionOffset.y+ 0.5f);
            }
        }

        private void Update()
        {
            if (_hoveringActivated && TryGetHoveredTile(out _hoveredCell))
            {
                _cursor.gameObject.SetActive(true);
                 Vector3 newCursorPosition = _hoveredCell.position;
                 newCursorPosition.y = 0.0001f;
                 _cursor.position = newCursorPosition;
            }
            else
            {
                _cursor.gameObject.SetActive(false);
            }
        }

        public static Vector3 GetCellCenter(Vector3Int cell)
        {
            Vector3 center = Instance._grid.GetCellCenterWorld(cell);
            center.y = 0;
            return center;
        }

        public static Vector3 GetCellCenter(CellData cellData)
        {
            return GetCellCenter(cellData.cell);
        }
        
        private void InitCellData()
        {
            _cellDatas = new CellData[_gridSize.x, _gridSize.y];
           
            for (int i = 0; i < _cellDatas.GetLength(0); i++)
            {
                for (int j = 0; j < _cellDatas.GetLength(1); j++)
                {
                    _cellDatas[i, j] = new CellData(new Vector3Int(i + _gridRect.xMin, j + _gridRect.yMin));
                }
            }

            foreach (var cellData in _cellDatas)
            {
                cellData.FindNeighbours();
            }
        }

        public static CellData[] findNeighbours(CellData originCell)
        {
            List<CellData> neighbours = new List<CellData>();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    Vector3Int neighbour = originCell.cell + new Vector3Int(i, j);
                    if(i == 0 && j == 0 || CellIsOutOfGrid(neighbour)) continue;
                    neighbours.Add(GetCellDataFromCellId(neighbour));
                }
            }

            return neighbours.ToArray();
        }
        
        private bool TryGetHoveredTile(out CellData hoveredCell)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
            mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);
            
            Ray mouseRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit hit, 100, LayerMask.GetMask("Terrain")))
            {
                Vector3Int hoveredCellIndex = _grid.WorldToCell(hit.point);
                hoveredCellIndex.x = Mathf.Clamp(hoveredCellIndex.x, Instance._gridRect.xMin, Instance._gridRect.xMax - 1);
                hoveredCellIndex.y = Mathf.Clamp(hoveredCellIndex.y, Instance._gridRect.yMin, Instance._gridRect.yMax - 1);

                hoveredCell = GetCellDataFromCellId(hoveredCellIndex);
                
                return true; 
            }

            hoveredCell = null;
            return false;
        }

        public static void ResetTileMap(TileMapType tileMapType)
        {
            Tilemap resetedMap = GetTilemap(tileMapType);

            Vector3Int cursor = Vector3Int.zero;

            for (int y = Instance._gridRect.yMin; y < Instance._gridRect.yMax; y++)
            {
                for (int x = Instance._gridRect.xMin; x < Instance._gridRect.xMax; x++)
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
                TileMapType.Terrain => Instance._terrainTileMap,
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

        public static void PaintCursor(Color color)
        {
            Instance._cursorMat.color = color;
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
            return cell.x < Instance._gridRect.xMin || cell.y < Instance._gridRect.yMin ||
                    cell.x > Instance._gridRect.xMax - 1 || cell.y > Instance._gridRect.yMax - 1;
        }
        
        public static bool CellIsBlocked(Vector3Int cell)
        {
            return GetCellDataFromCellId(cell).isBlocked;
        }

        public static CellData GetCellDataFromCellId(Vector3Int cell)
        {
            //print($"{cell} : {cell.x - Instance._gridRect.xMin}, {cell.y - Instance._gridRect.yMin}");
            return GetCellDataFromIndex(cell.x - Instance._gridRect.xMin, cell.y - Instance._gridRect.yMin);
        }

        public static CellData GetCellDataFromIndex(Vector2Int index)
        {
            return GetCellDataFromIndex(index.x, index.y);
        }

        public static CellData GetCellDataFromIndex(int indexX, int indexY)
        {
            return Instance._cellDatas[indexX, indexY];
        }

        public static void MonthUpdateCells()
        {
            foreach (var cell in Instance._cellDatas)
            {
                cell.OnMonthUpdate();
            }
        }
    }
}