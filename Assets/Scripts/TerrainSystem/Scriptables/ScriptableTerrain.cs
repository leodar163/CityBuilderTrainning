using UnityEngine;

namespace TerrainSystem.Scriptables
{
    [CreateAssetMenu(fileName = "NewTerrain", menuName = "Terrains/Terrain")]
    public class ScriptableTerrain : ScriptableObject
    {
        [SerializeField] private TerrainType _terrain;
        public TerrainType terrain => _terrain;
    }
}