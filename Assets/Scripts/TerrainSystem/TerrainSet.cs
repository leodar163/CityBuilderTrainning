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
        private TerrainType[] _terrains;

        public TerrainType[] terrains
        {
            get
            {
                if (_terrains == null || _terrains.Length != _terrainScripts.Length)
                {
                    _terrains = new TerrainType[_terrainScripts.Length];

                    for (int i = 0; i < _terrainScripts.Length; i++)
                    {
                        _terrains[i] = _terrainScripts[i].terrain;
                    }
                }

                return _terrains;
            }
        }
    }
}