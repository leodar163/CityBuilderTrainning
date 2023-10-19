using System;
using System.Collections.Generic;
using TerrainSystem.Scriptables;
using UnityEngine;
using Utils;

namespace TerrainSystem
{
    [CreateAssetMenu(menuName = "Terrains/Terrain Set", fileName = "NewTerrainSet")]
    public class TerrainSet : DefaultableScriptableObject<TerrainSet>
    {
        [SerializeField] private ScriptableTerrain[] _terrainScripts;

        public int nbrOfTerrain => _terrainScripts.Length;
        
        public TerrainType GetTerrain(int index)
        {
            if (index < 0 || index > _terrainScripts.Length) return null;

            return _terrainScripts[index].terrain;
        }

        public TerrainType GetTerrain(string id)
        {
            foreach (var scTerrain in _terrainScripts)
            {
                if (scTerrain.terrain.id == id) return scTerrain.terrain;
            }

            return null;
        }
    }
}