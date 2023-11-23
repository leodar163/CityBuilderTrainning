using UnityEngine;

namespace BuildingSystem.Facilities.Scriptables
{
    public abstract class ScriptableFacility : ScriptableObject
    {
        public virtual FacilityType Facility { get; }
        
        public virtual FacilityType GetFacilityCopy()
        {
            return null;
        }
    }
}