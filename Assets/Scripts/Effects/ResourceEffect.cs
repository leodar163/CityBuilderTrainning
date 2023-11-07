using System.Collections.Generic;
using ResourceSystem.Markets;

namespace Effects
{
    public class ResourceEffect : Effect, IEconomicActor
    {
        public string EconomicActorName => EffectName;
        public Market market { get; }

        
        
        List<ResourceOrder> IEconomicActor.orders { get; } = new();

        public IEconomicActor economicActorSelf { get; }

        public override void Apply()
        {
            base.Apply();
            
        }
    }
}