using System;
using System.Collections.Generic;
using BuildingSystem.Facilities.Scriptables;

namespace BuildingSystem.Facilities.HealthManagement
{
    public interface IGrowthEffector
    {
        public IGrowthEffector GrowthEffectorSelf { get; }
        
        public string GrowthEffectorName { get; }
        public float Growth { get; }

        protected List<ScriptableFacility> _inclusiveFilter { get; }
        protected List<ScriptableFacility> _exclusiveFilter { get; }

        public Action<IGrowthEffector> OnDestroyed { get; set; }
        
        public void ApplyEffector(FacilityType facility)
        {
            if (_inclusiveFilter.Count > 0)
            {
                foreach (var filter in _inclusiveFilter)
                {
                    if (filter.Facility.IsSameType(facility))
                        goto ADDING;
                }
                return;
            }

            foreach (var filter in _exclusiveFilter)
            {
                if (filter.Facility.IsSameType(facility)) return;
            }

            ADDING:
            facility.AddGrowthEffector(this);
        }

        public void Destroy()
        {
            OnDestroyed?.Invoke(this);
        }
    }
}