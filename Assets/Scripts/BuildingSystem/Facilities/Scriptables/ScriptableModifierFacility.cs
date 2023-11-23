using BuildingSystem.Facilities.FacilityTypes;
using UnityEngine;

namespace BuildingSystem.Facilities.Scriptables
{
    [CreateAssetMenu(menuName = "Facilities/Market Modifier", fileName = "NewModifierFacility")]
    public class ScriptableModifierFacility : ScriptableFacility
    {
        [SerializeField] private ModifierFacility _facility;
        
        public override FacilityType Facility => _facility;
        
        public override FacilityType GetFacilityCopy()
        {
            return _facility.Copy();
        }
    }
}