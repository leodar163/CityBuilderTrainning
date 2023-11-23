using BuildingSystem.Facilities.FacilityTypes;
using UnityEngine;

namespace BuildingSystem.Facilities.Scriptables
{
    [CreateAssetMenu(menuName = "Facilities/Auto Replicant", fileName = "NewAutoReplicantFacility")]
    public class ScriptableAutoReplicantFacility : ScriptableFacility
    {
        [SerializeField] private AutoReplicantFacility _facility;

        public override FacilityType Facility => _facility;

        public override FacilityType GetFacilityCopy()
        {
            return _facility.Copy();
        }
    }
}