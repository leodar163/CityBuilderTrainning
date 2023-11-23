using System.Collections.Generic;
using BuildingSystem.Facilities.Scriptables;
using UnityEngine;
using Utils;

namespace BuildingSystem.Facilities
{
    [CreateAssetMenu(menuName = "Facilities/Facility Set", fileName = "NewFacilitySet")]
    public class FacilitySet : DefaultableScriptableObject<FacilitySet>
    {
        public List<ScriptableFacility> facilities;
    }
}