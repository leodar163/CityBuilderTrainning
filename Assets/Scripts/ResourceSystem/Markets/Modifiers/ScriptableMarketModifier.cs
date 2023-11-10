using Effects;
using UnityEngine;

namespace ResourceSystem.Markets.Modifiers
{
    [CreateAssetMenu(menuName = "Effects/MarketModifier", fileName = "NewMarketModifier")]
    public class ScriptableMarketModifier : ScriptableEffect<MarketModifier>
    {
        public override MarketModifier GetEffectCopy()
        {
            return new MarketModifier(_effect);
        }
    }
}