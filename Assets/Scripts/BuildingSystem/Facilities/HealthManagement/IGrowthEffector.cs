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

        public List<FacilityType> facilities { get; }

        public void ApplyEffector(FacilityType facility)
        {
            if (_inclusiveFilter is {Count: > 0})
            {
                foreach (var filter in _inclusiveFilter)
                {
                    if (filter.Facility.IsSameType(facility))
                        goto ADDING;
                }
                return;
            }

            if (_exclusiveFilter is {Count: > 0})
            {
                foreach (var filter in _exclusiveFilter)
                {
                    if (filter.Facility.IsSameType(facility)) return;
                }
            }
            
            ADDING:
            facilities.Add(facility);
            facility.AddGrowthEffector(this);
        }

        public void UnApplyEffector(FacilityType facility)
        {
            if (!facilities.Remove(facility)) return;
            
            facility.RemoveGrowthEffector(this);
        }
        
        public void Destroy()
        {
            foreach (var facility in facilities)
            {
                facility.RemoveGrowthEffector(this);
            }
            facilities.Clear();
        }
    }
}