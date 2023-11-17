using System;
using TerrainSystem.Scriptables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GridSystem.Generation
{
    [Serializable]
    public struct TerrainLevel
    {
        public ScriptableTerrain terrain;
        [Range(0, 1)] public float threshold;
        public float seed;

        public TerrainLevel(ScriptableTerrain terrain, float threshold)
        {
            this.terrain = terrain;
            this.threshold = Mathf.Clamp01(threshold);
            seed = Random.Range(0, 1000);
        }
    }
}