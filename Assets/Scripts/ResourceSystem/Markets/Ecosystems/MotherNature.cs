using TerrainSystem;
using TimeSystem;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace ResourceSystem.Markets.Ecosystems
{
    public class MotherNature : Singleton<MotherNature>
    {
        [SerializeField] private EcosystemData[] _ecosystemsData;

        private void OnEnable()
        {
            TimeManager.onMonthEnds += MakeADecision;
        }

        private void OnDisable()
        {
            TimeManager.onMonthEnds -= MakeADecision;
        }

        private void MakeADecision()
        {
            Market randomEcosystem =
                MarketManager.ecosystemMarkets[Random.Range(0, MarketManager.ecosystemMarkets.Count)];

            TerrainType ecosystemType = randomEcosystem.cells[0].terrain;
        }

        private static EcosystemData GetEcosystemDataFromTerrain(TerrainType terrain)
        {
            foreach (var data in Instance._ecosystemsData)
            {
                if (data.terrain == terrain) return data;
            }

            return default;
        }
    }
}