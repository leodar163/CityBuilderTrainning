using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;
using Random = UnityEngine.Random;

namespace GridSystem.Generation
{
    public class MapGenerator : Singleton<MapGenerator>
    {
        [SerializeField] private TileBase _defaultTile;
        [SerializeField] private TileBase _landTile;
        [SerializeField] private Tilemap _previewTileMap;
        [SerializeField] private Vector2Int _mapSize;
        [Header("Borders")] 
        [SerializeField] private bool _showBorderDebug = true;
        [SerializeField] [Min(2)] private int _definition = 12;
        [SerializeField] [Range(0,1)] private float _attraction = 0;
        [SerializeField] private float _rotationOffset;
        [SerializeField] private float _outerRadius;
        [SerializeField] private float _innerRadius;

        private Vector3[] _borderPositions;

        private void OnDrawGizmos()
        {
            /*
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, Vector3.right * 5);
            Gizmos.DrawLine(Vector3.zero, Vector3.forward * 5);
            Gizmos.DrawLine(Vector3.forward * 5, Vector3.right * 5);
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position, 0.5f);
            return;
            */
            if (!_showBorderDebug || Application.isPlaying) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _innerRadius);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _outerRadius);
            Gizmos.color = Color.magenta;
            for (int i = 0; i < _borderPositions.Length; i+=3)
            {
                Gizmos.DrawLine(_borderPositions[i], _borderPositions[1]);
                Gizmos.DrawLine(_borderPositions[i + 1], _borderPositions[i + 2]);
                Gizmos.DrawLine(_borderPositions[i + 2], _borderPositions[i]);
            }
        }

        private void OnValidate()
        {
            GridManager.GridSize = _mapSize;
            _previewTileMap.ClearAllTiles();
            //ClearMap(true);
            GenerateBorders();
            GenerateTerrain();
        }

        private void Start()
        {
            return;
            ClearMap();
            GenerateBorders();
            
        }

        private void ClearMap(bool asPreview = false)
        {
            if (_defaultTile == null) return;
            
            _previewTileMap.ClearAllTiles();
            
            for (int j = 0; j < _mapSize.y; j++)
            {
                for (int i = 0; i < _mapSize.x; i++)
                {
                    if (asPreview)
                    {
                        _previewTileMap.SetTile(GridManager.GetCoordinatesByCellIndex(i,j), _defaultTile);
                    }
                    else
                        GridManager.PaintTilemap(_defaultTile, TileMapType.Terrain, 
                        Color.white, new Vector3Int(i, j, 0));
                }
            }
        }

        private void GenerateBorders()
        {
            _borderPositions = new Vector3[_definition * 3];
            
            float definitionAngle = Mathf.PI * 2 / _definition;
           

            for (int i = 0; i < _definition ; i++)
            {
                _borderPositions[i * 3] = transform.position;

                if (i == 0)
                {
                    _borderPositions[i * 3 + 1] = new Vector3
                    {
                        x = Mathf.Cos(definitionAngle * i + _rotationOffset),
                        z = Mathf.Sin(definitionAngle * i + _rotationOffset)
                    } * Random.Range(_innerRadius, _outerRadius);
                }
                else
                {
                    _borderPositions[i * 3 + 1] = _borderPositions[(i - 1) * 3 + 2];
                }

                if (i == _definition - 1)
                {
                    _borderPositions[i * 3 + 2] = _borderPositions[1];
                }
                else
                {
                    float distance = Mathf.Lerp(Random.Range(_innerRadius, _outerRadius),
                        Vector3.Distance(_borderPositions[0], _borderPositions[i * 3 + 1]), _attraction);
                    
                    _borderPositions[i * 3 + 2] = new Vector3
                    {
                        x = Mathf.Cos(definitionAngle * (i + 1) + _rotationOffset),
                        z = Mathf.Sin(definitionAngle * (i + 1) + _rotationOffset)
                    } * distance;
                }
            }
        }

        private void GenerateTerrain()
        {
            for (int j = 0; j < _mapSize.y; j++)
            {
                for (int i = 0; i < _mapSize.x; i++)
                {
                    Vector3Int coordinates = GridManager.GetCoordinatesByCellIndex(i, j);
                    Vector3 planeCoordinates = new Vector3Int(coordinates.x, 0, coordinates.y);
                    
                    _previewTileMap.SetTile(coordinates, _landTile);

                    if (IsInBorders(planeCoordinates))
                    {
                        _previewTileMap.SetTile(coordinates, _landTile);
                    }
                    else
                    {
                        _previewTileMap.SetTile(coordinates, _defaultTile);
                    }
                }
            }
        }

        private bool IsInBorders(Vector3 position)
        {
            //Debug.DrawLine(position, position  + Vector3.up, Color.magenta, 10f);
            
            for (int i = 0; i < _definition; i++)
            {
                if (PointInTriangle(_borderPositions[i * 3], _borderPositions[i * 3 + 1], 
                        _borderPositions[i * 3 + 2], position))
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool PointInTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p)
        {
            return IsSameSide(p, a, b, c) && IsSameSide(p, b, a, c) && IsSameSide(p, c, a, b);
        }

        private bool IsSameSide(Vector3 point1, Vector3 point2, Vector3 a, Vector3 b)
        {
            Vector3 crossProd1 = Vector3.Cross(b - a, point1 - a);
            Vector3 crossProd2 = Vector3.Cross(b - a, point2 - a);

            return Vector3.Dot(crossProd1, crossProd2) >= 0;
        }
    }
}