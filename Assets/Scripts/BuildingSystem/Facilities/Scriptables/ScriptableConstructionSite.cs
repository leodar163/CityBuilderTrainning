using UnityEngine;

namespace BuildingSystem.Facilities.Scriptables
{
    [CreateAssetMenu(menuName = "Facilities/Construction Site", fileName = "NewConstructionSite")]
    public class ScriptableConstructionSite : ScriptableFacility
    {
        [SerializeField] private ConstructionSite _constructionSite;
        public override FacilityType Facility => _constructionSite;
        public override FacilityType GetFacilityCopy()
        {
            return _constructionSite.Copy();
        }
    }
}