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
        [HideInInspector] public float efficiency;

        public string lineName => _localizedName.GetLocalizedString();

        [Space]
        public List<ResourceProduction> inputs;

        public List<ResourceProduction> outputs;

        public ProductionLine(List<ResourceProduction> inputs, List<ResourceProduction> outputs)
        {
            this.inputs = inputs;
            this.outputs = outputs;
        }
    }

    [Serializable]
    public struct ResourceProduction
    {
        public ResourceType resource;
        public float amount;
        public bool isLoan;

        public ResourceProduction(ResourceType resource, float amount, bool isLoan = false)
        {
            this.resource = resource;
            this.amount = amount;
            this.isLoan = isLoan;
        }
    }
}