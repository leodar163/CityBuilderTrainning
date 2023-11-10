using Effects;
using UnityEngine;

namespace ResourceSystem.Markets.Modifiers
{
    [CreateAssetMenu(menuName = "Effects/MarketModifier", fileName = "NewMarketModifier")]
    public class ScriptableMarketModifier : ScriptableEffect
    {
        [SerializeField] private MarketModifier _marketModifier;
        public override Effect GetEffectCopy()
        {
            return new MarketModifier(_marketModifier);
        }
    }
}