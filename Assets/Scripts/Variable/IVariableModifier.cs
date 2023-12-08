using System.Collections.Generic;

namespace Variable
{
    public interface IVariableModifier
    {
        public IVariableModifier VariableModifierSelf { get; }
        
        public float Add { get; }
        public float Mult { get; }
        public float Percent { get; }

        protected List<SubscribableFloat> variables { get; }

        public void ApplyTo(SubscribableFloat variable)
        {
            if (variables.Contains(variable)) return;
            
            variables.Add(variable);
            variable.AddModifier(this);
            
        }

        public void RemoveFrom(SubscribableFloat variable)
        {
            if (!variables.Remove(variable)) return;
            
            variable.RemoveModifier(this);
        }

        public void RemoveFromAllVariable()
        {
            while (variables.Count > 0)
            {
                RemoveFrom(variables[0]);
            }
        }
    }
}