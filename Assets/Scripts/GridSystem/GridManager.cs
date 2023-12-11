using System;
using System.Collections.Generic;
using ResourceSystem.Markets;
using TimeSystem;
using UnityEngine;
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
        [SerializeField] private Tilemap _marketVueTileMap;
       
        [Header("Bounds")] 
        [SerializeField] private Vector2Int _gridSize = new (62, 62); 
        [SerializeField] private Vector2Int _positionOffset;

        private CellData[,] _cellsData; 
      
        private Camera _mainCamera;

        [HideInInspector] [SerializeField] private RectInt _gridRect;

        private bool _gridInitializationCompleted;

        public static CellData[,] CellsDataCopy => Instance._cellsData.Clone() as CellData[,];
        
        public static Vector2Int GridSize
        {
            get => Instance._gridSize;
            set
            {
                Instance._gridSize = value;
                Instance._gridRect =
                    new RectInt(-Instance._gridSize / 2 + Instance._positionOffset, Instance._gridSize);
                Instance.InitCollider();
            }
        }

        public static bool GridInitializationCompleted => Instance._gridInitializationCompleted;

        public static RectInt GridRect => Instance._gridRect;
        
        private void Awake()
        {
            TimeManager.onNewMonth += MonthUpdateCells;
            
            _mainCamera = Camera.main;
            InitCellsData();
        }

       
        private void OnValidate()
        {
            if (!_gridCollider) TryGetComponent(out _gridCollider);
            InitCollider();
        }

        private void InitCollider()
        {
            if (_gridCollider)
            {
                _gridCollider.size = new Vector3(_gridSize.x, 0.01f,_gridSize.y);
                _gridCollider.center = new Vector3(_positionOffset.x, 0, _positionOffset.y);
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
            return GetCellCenter(cellData.coordinates);
        }
        
        private void InitCellsData()
        {
            _cellsData = new CellData[_gridSize.x, _gridSize.y];
           
            for (int i = 0; i < _cellsData.GetLength(0); i++)
            {
                for (int j = 0; j < _cellsData.GetLength(1); j++)
                {
                    _cellsData[i, j] = new CellData(GetCoordinatesByCellIndex(i,j));
                }
            }

            foreach (var cellData in _cellsData)
            {
                cellData.FindNeighbours();
            }

            _gridInitializationCompleted = true;

            // foreach (var cellData in _cellsData)
            // {
            //     MarketManager.AddMarket(MarketType.Ecosystem, cellData);
            // }
        }

        public static Vector3Int GetCoordinatesByCellIndex(Vector2Int indices)
        {
            return GetCoordinatesByCellIndex(indices.x, indices.y);
        }
        
        public static Vector3Int GetCoordinatesByCellIndex(int x, int y)
        {
            return new Vector3Int(x + Instance._gridRect.xMin, y + Instance._gridRect.yMin);
        }
        
        public static CellData[] GetOuterBorderOfArea(List<CellData> area)
        {
            List<CellData> border = new();

            foreach (var cell in area)
            {
                CellData[] neighbours = cell.neighbours;

                foreach (var neighbour in neighbours)
                {
                    if (!area.Contains(neighbour) && !border.Contains(neighbour))
                    {
                        border.Add(neighbour);
                    }
                }
            }

            return border.ToArray();
        }
        
        public static CellData[] GetInnerBorderOfArea(List<CellData> area)
        {
            List<CellData> border = new();

            foreach (var cell in area)
            {
                if (cell.neighbours.Length < 8)
                {
                    border.Add(cell);
                    continue;
                }
                
                CellData[] neighbours = cell.neighbours;

                foreach (var neighbour in neighbours)
                {
                    if (!area.Contains(neighbour))
                    {
                        border.Add(cell);
                        break;
                    }
                }
            }

            return border.ToArray();
        }

        public static float GetMinDistanceBetweenAreas(CellData[] areaA, CellData[] areaB)
        {
            float distance = float.PositiveInfinity;

            foreach (var borderA in areaA)
            {
                foreach (var borderB in areaB)
                {
                    float currentDistance = CellData.Distance(borderA, borderB);
                    if (currentDistance < distance) distance = currentDistance;
                    if (currentDistance == 0) goto Return;
                }
            }
            
            Return:
            return distance;
        }
        
        /// <summary>
        /// Find neighbours in range using cell indices of the grid.
        /// If CellData are already initialized, use GetNeighbours instead.
        /// </summary>
        /// <returns></returns>
        public static CellData[] FindNeighbours(CellData originCell, int range = 1, bool includeOrigin = false, bool onlyAdjacent = false)
        {
            switch (range)
            {
                case 0 when !includeOrigin:
                case < 0:
                    return null;
                case 0:
                    return new[] { originCell };
            }

            List<CellData> neighbours = new List<CellData>();

            for (int i = -range; i <= range; i++)
                for (int j = -range; j <= range; j++)
                {
                    if (i == 0 && j == 0 && !includeOrigin) continue;
                    
                    Vector3Int neighbourId = originCell.coordinates + new Vector3Int(i, j);

                    if (onlyAdjacent && neighbourId.x != originCell.coordinates.x &&
                        neighbourId.y != originCell.coordinates.y)
                        continue;
                    
                    if (TryGetCellDataFromCellId(neighbourId,
                            out CellData neighbour))
                    {
                        neighbours.Add(neighbour);
                    }
                }

                return neighbours.ToArray();
        }

        /// <summary>
        /// Find neighbours in range using neighbour list stored in CellData's properties.
        /// If CellData are not initialized yet, use FindNeighbours instead.
        /// </summary>
        /// <returns></returns>
        public static CellData[] GetNeighbours(CellData originCell, int range = 1, bool includeOrigin = false)
        {
            switch (range)
            {
                case 0 when !includeOrigin:
                case < 0:
                    return null;
                case 0:
                    return new[] { originCell };
            }

            List<CellData> neighbours = new();
            Queue<CellData> openSet = new();

            if (includeOrigin) neighbours.Add(originCell);
            openSet.Enqueue(originCell);

            while (range > 0 && openSet.Count > 0)
            {
                CellData cursor = openSet.Dequeue();

                foreach (var neighbour in cursor.neighbours)
                {
                    if (neighbours.Contains(neighbour)) continue;

                    neighbours.Add(neighbour);
                    openSet.Enqueue(neighbour);
                }

                range--;
            }

            return neighbours.ToArray();
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
                TileMapType.Market => Instance._marketVueTileMap,
                _ => throw new ArgumentOutOfRangeException(nameof(tileMapType), tileMapType, null)
            };
        }
        
        public static void PaintTilemap(TileBase paintTile, TileMapType tilemap, params Vector3Int[] cellsToPaint)
        {
            PaintTilemap(paintTile, tilemap, Color.white, cellsToPaint);
        }
        
        public static void PaintTilemap(TileBase paintTile, TileMapType tilemap, Color tint, params Vector3Int[] cellsToPaint)
        {
            Tilemap mapToPaint = GetTilemap(tilemap);
            foreach (var cell in cellsToPaint)
            {
                PaintTile(paintTile, mapToPaint, tint, cell);
            }
        }

        public static void PaintTilemap(TileBase paintTile, TileMapType tilemap, params CellData[] area)
        {
            PaintTilemap(paintTile, tilemap, Color.white, area);
        }
        
        
        public static void PaintTilemap(TileBase paintTile, TileMapType tilemap, Color tint, params CellData[] area)
        {
            Tilemap mapToPaint = GetTilemap(tilemap);
            foreach (var cell in area)
            {
                PaintTile(paintTile, mapToPaint, tint, cell.coordinates);
            }
        }

        private static void PaintTile(TileBase paintTile, Tilemap tilemap, Color tint, Vector3Int cellIndex)
        {
            if (CellIsOutOfGrid(cellIndex)) return;
            tilemap.SetTile(cellIndex, paintTile);
            tilemap.SetTileFlags(cellIndex, TileFlags.None);
            tilemap.SetColor(cellIndex, tint);
        }

        public static bool CellIsOutOfGrid(Vector3Int cell)
        {
            return cell.x < Instance._gridRect.xMin || cell.y < Instance._gridRect.yMin ||
                    cell.x > Instance._gridRect.xMax - 1 || cell.y > Instance._gridRect.yMax - 1;
        }
        
        public static bool TryGetCellDataFromCellId(Vector3Int cell, out CellData cellData)
        {
            //print($"{cell} : {cell.x - Instance._gridRect.xMin}, {cell.y - Instance._gridRect.yMin}");
            if (CellIsOutOfGrid(cell))
            {
                cellData = null;
                return false;
            }
            
            cellData = GetCellDataFromIndex(cell.x - Instance._gridRect.xMin, cell.y - Instance._gridRect.yMin);
            return true;
        }

        public static bool TryGetCellDataFromMousePosition(out CellData hoveredCell)
        {
            Ray mouseRay = Instance._mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit hit, 100, LayerMask.GetMask("Terrain")))
            {
                Vector3Int hoveredCellIndex = Instance._grid.WorldToCell(hit.point);
                hoveredCellIndex.x = Mathf.Clamp(hoveredCellIndex.x, Instance._gridRect.xMin, Instance._gridRect.xMax - 1);
                hoveredCellIndex.y = Mathf.Clamp(hoveredCellIndex.y, Instance._gridRect.yMin, Instance._gridRect.yMax - 1);

                if (TryGetCellDataFromCellId(hoveredCellIndex, out hoveredCell))
                    return true; 
            }

            hoveredCell = null;
            return false;
        }

        public static CellData GetCellDataFromIndex(Vector2Int index)
        {
            return GetCellDataFromIndex(index.x, index.y);
        }

        public static CellData GetCellDataFromIndex(int indexX, int indexY)
        {
            return Instance._cellsData[indexX, indexY];
        }

        public static void MonthUpdateCells()
        {
            foreach (var cell in Instance._cellsData)
            {
                cell.OnMonthUpdate();
            }
        }

        public static void ShowTileMap(TileMapType tileMapType, bool show)
        {
            GetTilemap(tileMapType).gameObject.SetActive(show);
        }
    }
}