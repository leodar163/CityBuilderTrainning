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
            Market,
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
            InitCellDatas();
        }

        /*
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(new Vector3(_positionOffset.x + 0.5f, 0, _positionOffset.y + 0.5f),new Vector3(_gridSize.x, 0.01f, _gridSize.y));
        }
        */
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
                _hoveredCell = null;
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
            return GetCellCenter(cellData.cellCoordinates);
        }
        
        private void InitCellDatas()
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
                MarketManager.AddMarket(true, cellData);
            }
        }

        public static CellData[] GetOuterBorderOfArea(List<CellData> area)
        {
            List<CellData> border = new();

            foreach (var cell in area)
            {
                CellData[] neighbours = cell.neighbours;

                foreach (var neighbour in neighbours)
                {
                    if (!area.Contains(neighbour))
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
        public static CellData[] FindNeighbours(CellData originCell, int range = 1, bool includeOrigin = false)
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

                    if (TryGetCellDataFromCellId(originCell.cellCoordinates + new Vector3Int(i, j),
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

                if (TryGetCellDataFromCellId(hoveredCellIndex, out hoveredCell))
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
                Vector3Int cellIndex = cell.cellCoordinates;
                PaintTile(paintTile, mapToPaint, tint, cellIndex);
            }
        }

        public static void PaintTile(TileBase paintTile, Tilemap tilemap, Color tint, Vector3Int cellIndex)
        {
            if (CellIsOutOfGrid(cellIndex)) return;
            
            tilemap.SetTile(cellIndex, paintTile);
            tilemap.SetTileFlags(cellIndex, TileFlags.None);
            tilemap.SetColor(cellIndex, tint);
        }
        
        public static void PaintCursor(Color color)
        {
            Instance._cursorMat.color = color;
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

        public static void ShowTileMap(TileMapType tileMapType, bool show)
        {
            GetTilemap(tileMapType).gameObject.SetActive(show);
        }
    }
}