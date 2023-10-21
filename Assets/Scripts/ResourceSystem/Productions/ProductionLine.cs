using System;
using System.Collections.Generic;
using ResourceSystem.Markets;
using UnityEngine;

namespace ResourceSystem.Productions
{
    [Serializable]
    public class ProductionLine
    {
        public string name;
        private float _efficiency = 1;
        public bool unrestSensitive = true;

        public float efficiency => _efficiency;
        
        public List<ResourceQuantity> demands = new();
        public List<ResourceQuantity> offers = new();
        
        public void SetEfficiency(Market market)
        {
            _efficiency = 1;
            
            foreach (var demand in demands)
            {
                float availability = Mathf.Clamp(market.GetResourceAvailability(demand.resource), -1, 1);
                if (availability <= 0)
                {
                    _efficiency = 0;
                    return;
                }
                if (availability < _efficiency) _efficiency = availability;
            }

            if (market._peopleNeedManager != null && unrestSensitive)
            {
                _efficiency -= market._peopleNeedManager.unrest / 2;
            }
            
        }
    }
}