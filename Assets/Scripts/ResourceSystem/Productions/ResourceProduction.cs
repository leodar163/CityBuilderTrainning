using System;
using UnityEngine;

namespace ResourceSystem.Productions
{
    [Serializable]
    public class ResourceProduction
    {
        public ResourceType resource;
        public float amount;
        [HideInInspector] public float expectedAmount;
        public bool isLoan;

        public ResourceProduction(ResourceType resource, float amount, bool isLoan = false)
        {
            this.resource = resource;
            this.amount = amount;
            this.isLoan = isLoan;
        }
    }
}