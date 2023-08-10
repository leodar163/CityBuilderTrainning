using TerrainSystem;
using Utils;

namespace BuildingSystem.Facilities
{
    public class FacilityPlacer : Singleton<FacilityPlacer>
    {
        public static void PlaceNewFacility(Facility facilityToPlace, TerrainData terrain)
        {
            if (terrain.freeFacilityPlacements > 0 && facilityToPlace.CanBePlaced(terrain))
            {
                
            }
        }
    }
}