using System;
using GridSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Random = UnityEngine.Random;

namespace TerrainSystem
{
    public class TerrainBuilder : Singleton<TerrainBuilder>
    {
        [SerializeField] private TerrainSet _terrainSet;
        private int[,] _terrainMap;

        private void Start()
        {
            if (_terrainSet == null) _terrainSet = TerrainSet.Default;
            BuildTerrain(GenerateRandomTerrainMap(GridManager.GridSize));
        }

        public static int[,] GenerateRandomTerrainMap(Vector2Int mapSize)
        {
            int[,] terrainMap = new int[mapSize.x, mapSize.y];
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    terrainMap[x, y] = Random.Range(0, Instance._terrainSet.terrains.Length);
                }
            }

            return terrainMap;
        }
        
        public static void BuildTerrain(int[,] terrainMap)
        {
            Vector2Int mapSize = GridManager.GridSize;
            Instance._terrainMap = new int[mapSize.x, mapSize.y];

            for (int y = 0; y < Instance._terrainMap.GetLength(1); y++)
            {
                for (int x = 0; x < Instance._terrainMap.GetLength(0); x++)
                {
                    if (terrainMap.GetLength(0) < x || terrainMap.GetLength(1) < y)
                        continue;

                    int terrainIndex = terrainMap[x, y];
                    if(terrainIndex < 0 || terrainIndex > Instance._terrainSet.terrains.Length)
                        continue;
                        
                    Instance._terrainMap[x, y] = terrainIndex;

                    if (Instantiate(Instance._terrainSet.terrains[terrainIndex]).TryGetComponent(out TerrainData newTerrain))
                    {
                        newTerrain.OnAddedToCell(GridManager.GetCellDataFromIndex(x,y));
                    }
                }
            }
        }
    }
}