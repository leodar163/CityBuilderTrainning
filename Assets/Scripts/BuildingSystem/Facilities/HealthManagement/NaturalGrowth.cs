using System;
using System.Collections.Generic;
using BuildingSystem.Facilities.Scriptables;
using Localization;

namespace BuildingSystem.Facilities.HealthManagement
{
    public class NaturalGrowth : IGrowthEffector
    {
        public IGrowthEffector GrowthEffectorSelf => this;
        public string GrowthEffectorName => VariableNameManager.BaseName;
        public float Growth { get; set; }

        List<ScriptableFacility> IGrowthEffector._inclusiveFilter { get; }

        List<ScriptableFacility> IGrowthEffector._exclusiveFilter { get; }

        List<FacilityType> IGrowthEffector.facilities { get; } = new();

        public Action<IGrowthEffector> OnDestroyed { get; set; }
    }
}