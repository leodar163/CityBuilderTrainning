using System.Collections.Generic;
using Localization;

namespace ResourceSystem.Market
{
    public class MarketOption : IResourceModifier
    {
        private readonly List<ResourceDelta> deltas;
        public ResourceDeck deck { get; private set; }

        public MarketOption(ResourceDeck deck, List<ResourceDelta> deltas)
        {
            this.deltas = deltas;
            this.deck = deck;
        }

        public void AddDelta(ResourceDelta delta)
        {
            for (int i = 0; i < deltas.Count; i++)
            {
                if (deltas[i].resource == delta.resource)
                {
                    deltas[i] += delta;
                    return;
                }
            }
            deltas.Add(delta);
        }

        public void RemoveDelta(ResourceType resourceType)
        {
            for (int i = 0; i < deltas.Count; i++)
            {
                if (deltas[i].resource == resourceType)
                {
                    deltas.RemoveAt(i);
                    return;
                }
            }
        }
                
        public string modifierName => VariableNameManager.MarketName;
        public ResourceDelta[] GetResourceDelta()
        {
            return deltas.ToArray();
        }
    }
}