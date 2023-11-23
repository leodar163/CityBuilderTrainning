using BuildingSystem.Facilities.FacilityTypes;
using UnityEngine;

namespace BuildingSystem.Facilities.Scriptables
{
    [CreateAssetMenu(menuName = "Facilities/Market Creator", fileName = "NewMarketCreatorFacility")]
    public class ScriptableMarketCreatorFacility : ScriptableFacility
    {
        [SerializeField] private MarketCreatorFacility _facility;

        public override FacilityType Facility => _facility;
        
        public override FacilityType GetFacilityCopy()
        {
            return _facility.Copy();
        }
    }
}