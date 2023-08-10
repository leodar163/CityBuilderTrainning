using TerrainSystem;

namespace BuildingSystem.Facilities
{
    public class Tree : Facility
    {
        public override bool CanBePlaced(TerrainData terrain)
        {
            return terrain is Forest;
        }
    }
}