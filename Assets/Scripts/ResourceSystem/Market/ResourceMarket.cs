using System.Collections.Generic;
using UnityEngine;

namespace ResourceSystem.Market
{
    public class ResourceMarket
    {
        public ResourceDeck marketDeck { get; private set; }
        public readonly List<ResourceDeck> decks;
        public readonly Dictionary<ResourceDeck, List<ResourceDelta>> demands = new();
        public readonly Dictionary<ResourceType, float> surpluses = new();
        
        public ResourceMarket()
        {
            decks = new List<ResourceDeck>();
            marketDeck = new ResourceDeck(ResourceSet.Default);
        }
        
        public ResourceMarket(params ResourceDeck[] decks)
        {
            this.decks = new List<ResourceDeck>(decks);
            marketDeck = new ResourceDeck(ResourceSet.Default);
        }

        public float TryDemand(ResourceDeck deck, ResourceDelta delta)
        {
            float quantityToFind = delta.resource.borrowable ? delta.quantityDelta : delta.monthDelta;

            if (!decks.Contains(deck)) return quantityToFind;
            
            foreach (var deckMarket in decks)
            {
                if (deckMarket == deck) continue;
                ResourceSlider slider = deckMarket.GetSlider(delta.resource);
                if(slider.availableQuantity == 0) continue;
                
                float availableQuantity = Mathf.Clamp(slider.availableQuantity, 0, quantityToFind);

                if (delta.resource.borrowable)
                {
                }
            }

            return quantityToFind;
        }
        
        
    }
}