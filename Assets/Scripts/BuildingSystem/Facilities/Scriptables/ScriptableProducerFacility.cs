using BuildingSystem.Facilities.FacilityTypes;
using UnityEngine;

namespace BuildingSystem.Facilities.Scriptables
{
    [CreateAssetMenu(menuName = "Facilities/Producer", fileName = "NewProducerFacility")]
    public class ScriptableProducerFacility : ScriptableFacility
    {
        [SerializeField] private ProducerFacility _facility;
        
        public override FacilityType Facility => _facility;
        
        public override FacilityType GetFacilityCopy()
        {
            return _facility.Copy();
        }
    }
}