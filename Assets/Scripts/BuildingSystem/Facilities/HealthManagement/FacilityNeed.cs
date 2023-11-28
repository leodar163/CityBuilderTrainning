using System;
using System.Collections.Generic;
using BuildingSystem.Facilities.Scriptables;
using Format;
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

        List<ScriptableFacility> IGrowthEffector._inclusiveFilter { get; } = new();

        List<ScriptableFacility> IGrowthEffector._exclusiveFilter { get; } = new();

        public Action<IGrowthEffector> OnDestroyed { get; set; }

        private string FormatNeed()
        {
            if (resource == null) return "miss_resource_to_need";
            return string.Format(VariableNameManager.NeedFormat, resource.ResourceName);
        }
    }
}