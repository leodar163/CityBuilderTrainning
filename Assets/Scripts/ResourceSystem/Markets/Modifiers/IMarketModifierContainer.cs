using System.Collections.Generic;

namespace ResourceSystem.Markets.Modifiers
{
    public interface IMarketModifierContainer
    {
        public IMarketModifierContainer modifierContainerSelf { get; }
        public List<MarketModifier> modifiers { get;}

        protected List<ResourceQuantity> multipliers { get; }

        public float GetMultiplier(ResourceType resource)
        {
            foreach (var mult in multipliers)
            {
                if (mult.resource == resource)
                    return mult.quantity;
            }

            return 0;
        }

        private void AddMultiplier(ResourceType resource, float mult)
        {
            if (mult == 0 ) return;
            int index = -1;
            for (int i = 0; i < multipliers.Count; i++)
            {
                if (multipliers[i].resource == resource)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                multipliers.Add(new ResourceQuantity(resource, mult));
                return;
            }

            multipliers[index] += mult;

            if (multipliers[index].quantity == 0)
            {
                multipliers.RemoveAt(index);
            }
        }
        
        public void AddModifier(MarketModifier modifier)
        {
            if (modifiers.Contains(modifier)) return;
            
            modifiers.Add(modifier);

            foreach (var mult in modifier.Multipliers)
            {
                AddMultiplier(mult.resource, mult.quantity);
            }
        }
        
        public void RemoveModifier(MarketModifier modifier)
        {
            if (!modifiers.Contains(modifier)) return;

            modifiers.Remove(modifier);
            modifier.OnRemoved();

            foreach (var mult in modifier.Multipliers)
            {
                AddMultiplier(mult.resource, -mult.quantity);
            }
        }
    }
}