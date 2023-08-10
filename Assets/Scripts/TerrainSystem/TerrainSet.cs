using UnityEngine;
using Utils;

namespace TerrainSystem
{
    [CreateAssetMenu(menuName = "Terrains/Terrain Set", fileName = "NewTerrainSet")]
    public class TerrainSet : ScriptableWithDefault<TerrainSet>
    {
        [SerializeField] public TerrainData[] terrains;
    }
}