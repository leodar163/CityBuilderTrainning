using System.Collections.Generic;

namespace ResourceSystem.Markets.Modifiers
{
    public interface IMarketModifierContainer
    {
        public IMarketModifierContainer modifierContainerSelf { get; }
        public List<MarketModifier> modifiers { get; set; }

        public void AddModifier(MarketModifier modifier)
        {
            if (modifiers.Contains(modifier)) return;
            
            modifiers.Add(modifier);
        }
        
        public void RemoveModifier(MarketModifier modifier)
        {
            if (!modifiers.Contains(modifier)) return;

            modifiers.Remove(modifier);
            modifier.OnRemoved();
        }
    }
}