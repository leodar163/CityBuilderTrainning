using System.Collections.Generic;
using TerrainSystem;
using TerrainSystem.Scriptables;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;
using Utils.Maths;
using Random = UnityEngine.Random;

namespace GridSystem.Generation
{
    public class MapGenerator : Singleton<MapGenerator>
    {
        [SerializeField] private ScriptableTerrain _defaultTerrain;
        [SerializeField] private ScriptableTerrain _landTerrain;
        [SerializeField] private Tilemap _previewTileMap;
        [SerializeField] private Vector2Int _mapSize;
        [Header("Borders")] 
        [SerializeField] private bool _showBorderDebug = true;
        [SerializeField] [Min(2)] private int _definition = 12;
        [SerializeField] [Range(0,1)] private float _attraction = 0;
        [SerializeField] private float _rotationOffset;
        [SerializeField] private float _outerRadius;
        [SerializeField] private float _innerRadius;
        [Header("Terrains")] 
        [SerializeField] private Vector2 _noiseOffset;
        [SerializeField] private float _noiseFrequency = 1;
        [SerializeField] private float _noiseExponent = 1;
        [SerializeField] [Range(0,1)] private float _moistureScale = 0.5f;
        [SerializeField] private TerrainLevel[] _levels;

        private Vector3[] _borderPositions;
        private TerrainType[,] _terrainMap;

        public Texture2D noiseMap;
        [SerializeField] private int _shownLevel;

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
            GenerateBorders();
            //SortLevels();
            noiseMap = new Texture2D(_mapSize.x, _mapSize.y);

            _shownLevel = Mathf.Clamp(_shownLevel, 0, _levels.Length - 1);
            
            for (int j = 0; j < _mapSize.y; j++)
            {
                for (int i = 0; i < _mapSize.x; i++)
                {
                    noiseMap.SetPixel(i,j, Color.white * EvaluateTerrainNoise(i,j, _levels[_shownLevel].seed));
                }
            }
            
            noiseMap.Apply();
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
            SortLevels();

            _terrainMap = new TerrainType[_mapSize.x, _mapSize.y];
            
            for (int j = 0; j < _mapSize.y; j++)
            {
                for (int i = 0; i < _mapSize.x; i++)
                {
                    Vector3Int coordinates = GridManager.GetCoordinatesByCellIndex(i, j);
                    Vector3 planeCoordinates = new Vector3Int(coordinates.x, 0, coordinates.y);

                    if (IsInBorders(planeCoordinates))
                    {
                        //print($"{i}:{j} : {EvaluateTerrainNoise(i,j)}");

                        _terrainMap[i, j] = _landTerrain.terrain;
                        
                        foreach (var terrainLevel in _levels)
                        {
                            _terrainMap[i, j] = 
                                GetTileByByHeight(EvaluateTerrainNoise(i, j, terrainLevel.seed), terrainLevel, _terrainMap[i,j]);
                        }
                    }
                    else
                    {
                        _terrainMap[i, j] = _defaultTerrain.terrain;
                        
                    }
                    
                    _previewTileMap.SetTile(coordinates, _terrainMap[i,j].tile);
                }
            }
        }

        private float EvaluateTerrainNoise(float x, float y, float offset)
        {
            float noise = Mathf.Clamp01(Mathf.PerlinNoise(
                (x * _noiseFrequency + _noiseOffset.x + offset) / _mapSize.x ,
                (y * _noiseFrequency + _noiseOffset.y + offset) / _mapSize.y ));
            noise = Mathf.Pow(noise, _noiseExponent);

            float moisture = Mathf.Clamp01(Mathf.PerlinNoise(
                (x * _noiseFrequency * 10 + _noiseOffset.x + 100 + offset) / _mapSize.x ,
                (y * _noiseFrequency * 10+ _noiseOffset.y + 100 + offset) / _mapSize.y ));
            moisture = Mathf.Pow(moisture, _noiseExponent);

            noise = Mathf.Lerp(noise, moisture, _moistureScale);

            return noise;
        }
        
        private TerrainType GetTileByByHeight(float height, TerrainLevel terrainLevel, TerrainType defaultTerrain)
        {
            //print(height);
            height = Mathf.Clamp01(height);

            if (terrainLevel.threshold <= height) return terrainLevel.terrain.terrain;

            return defaultTerrain;
        }
        
        private bool IsInBorders(Vector3 position)
        {
            //Debug.DrawLine(position, position  + Vector3.up, Color.magenta, 10f);
            
            for (int i = 0; i < _definition; i++)
            {
                if (TrigoHelper.PointInTriangle(_borderPositions[i * 3], _borderPositions[i * 3 + 1], 
                        _borderPositions[i * 3 + 2], position))
                {
                    return true;
                }
            }

            return false;
        }

        private void SortLevels()
        {
            List<TerrainLevel> unSortedLevels = new List<TerrainLevel>(_levels);
            List<TerrainLevel> sortedLevels = new();
            
            while (sortedLevels.Count != _levels.Length)
            {
                TerrainLevel pickedTerrain = new TerrainLevel(null, Mathf.Infinity);

                foreach (var terrainLevel in unSortedLevels)
                {
                    if (terrainLevel.threshold < pickedTerrain.threshold)
                        pickedTerrain = terrainLevel;
                }
                sortedLevels.Add(pickedTerrain);
                unSortedLevels.Remove(pickedTerrain);
            }

            _levels = sortedLevels.ToArray();
        }

        public void GenerateMap(bool resetBorder = true)
        {
            GridManager.GridSize = _mapSize;
            _previewTileMap.ClearAllTiles();
            //ClearMap(true);
            if (_borderPositions.Length == 0 || resetBorder) GenerateBorders();
            GenerateTerrain();
        }
    }
}