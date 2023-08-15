using TerrainSystem;

namespace BuildingSystem.Facilities
{
    public class Tree : Facility
    {
        public override bool CanBePlaced(TerrainType terrain)
        {
            return terrain is Forest;
        }
    }
}