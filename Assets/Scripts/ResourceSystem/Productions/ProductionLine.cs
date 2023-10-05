using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace ResourceSystem.Productions
{
    [Serializable]
    public class ProductionLine
    {
        [SerializeField] private LocalizedString _localizedName;
        public float efficiency { get; private set; }
        
        public string lineName => _localizedName.GetLocalizedString();

        [Space]
        public List<ResourceProduction> inputs;

        public List<ResourceProduction> outputs;

        public ProductionLine(List<ResourceProduction> inputs, List<ResourceProduction> outputs)
        {
            this.inputs = inputs;
            this.outputs = outputs;
        }

        public void CalculateEfficiency()
        {
            foreach (var input in inputs)
            {
                float localEfficiency = input.amount / (input.amount - input.expectedAmount);
                if (localEfficiency < efficiency) efficiency = localEfficiency;
            }
        }
    }
}