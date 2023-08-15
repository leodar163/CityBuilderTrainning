using UnityEngine;
using Utils;

namespace TerrainSystem
{
    [CreateAssetMenu(menuName = "Terrains/Terrain Set", fileName = "NewTerrainSet")]
    public class TerrainSet : DefaultableScriptableObject<TerrainSet>
    {
        [SerializeField] public TerrainType[] terrains;
    }
}