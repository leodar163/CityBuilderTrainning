using System;
using System.Collections.Generic;
using BuildingSystem.Facilities.Scriptables;
using Localization;
using ResourceSystem;

namespace BuildingSystem.Facilities.HealthManagement
{
    public class FacilityNeed : IGrowthEffector
    {
        public ResourceType resource;
        public IGrowthEffector GrowthEffectorSelf => this;
        public string GrowthEffectorName => FormatNeed(); 
        public float Growth { get; set; }

        private FacilityType _facility;
        
        List<ScriptableFacility> IGrowthEffector._inclusiveFilter { get; } = new();

        List<ScriptableFacility> IGrowthEffector._exclusiveFilter { get; } = new();

        List<FacilityType> IGrowthEffector.facilities { get; } = new();

        public Action<IGrowthEffector> OnDestroyed { get; set; }

        private string FormatNeed()
        {
            if (resource == null) return "miss_resource_to_need";
            _facility ??= GrowthEffectorSelf.facilities[0];
            return string.Format(
                VariableNameManager.GetNeedFormat(_facility.cell.market.GetResourceAvailabilityState(resource)).String,
                resource.ResourceName);
        }
    }
}