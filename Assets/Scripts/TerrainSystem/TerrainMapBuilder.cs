using GridSystem;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace TerrainSystem
{
    public class TerrainMapBuilder : Singleton<TerrainMapBuilder>
    {
        [SerializeField] private TerrainSet _terrainSet;
        private int[,] _terrainMap;

        private void Start()
        {
            if (_terrainSet == null) _terrainSet = TerrainSet.Default;
            BuildTerrainMap(GenerateRandomTerrainMap(GridManager.GridSize));
            ApplyTerrainMap();
        }

        private int[,] GenerateRandomTerrainMap(Vector2Int mapSize)
        {
            int[,] terrainMap = new int[mapSize.x, mapSize.y];
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    terrainMap[x, y] = Random.Range(0, _terrainSet.terrains.Length);
                }
            }

            return terrainMap;
        }
        
        private void BuildTerrainMap(int[,] terrainMap)
        {
            Vector2Int mapSize = GridManager.GridSize;
            _terrainMap = new int[mapSize.x, mapSize.y];

            for (int y = 0; y < _terrainMap.GetLength(1); y++)
            {
                for (int x = 0; x < _terrainMap.GetLength(0); x++)
                {
                    if (terrainMap.GetLength(0) < x || terrainMap.GetLength(1) < y)
                        continue;

                    int terrainIndex = terrainMap[x, y];
                    if(terrainIndex < 0 || terrainIndex > _terrainSet.terrains.Length)
                        continue;
                        
                    _terrainMap[x, y] = terrainIndex;
                }
            }
        }

        private void ApplyTerrainMap()
        {
            for (int y = 0; y < _terrainMap.GetLength(1); y++)
            {
                for (int x = 0; x < _terrainMap.GetLength(0); x++)
                {
                    GridManager.GetCellDataFromIndex(x, y).SetTerrain(_terrainSet.terrains[_terrainMap[x,y]]);
                }
            }
           
        }
    }
}