using System;
using UnityEngine;

namespace ResourceSystem.Markets
{
    [Serializable]
    public struct OrderSummary
    {
        public ResourceType resource;
        [Min(0)] public float quantity;
        public OrderType orderType;
    }
}