using System.Collections.Generic;

namespace SubscribableVariables
{
    public interface IVariableModifier
    {
        public IVariableModifier VariableModifierSelf { get; }
        
        public float Add { get; protected set; }
        public float Mult { get; protected set;}
        public float AddPercent { get; protected set;}

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

        public void SetAdd(float newAdd)
        {
            Add = Add;
            NotifyVariables();
        }

        public void SetMult(float newMult)
        {
            Mult = newMult;
            NotifyVariables();
        }

        public void SetAddPercent(float newAddPercent)
        {
            AddPercent = newAddPercent;
            NotifyVariables();
        }

        private void NotifyVariables()
        {
            foreach (var variable in variables)
            {
                variable.RecalculateValue();
            }
        }
    }
}