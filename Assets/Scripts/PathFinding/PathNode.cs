using GridSystem;

namespace PathFinding
{
    public class PathNode
    {
        public CellData parent;
        
        public float realCost;
        public float heuristicCost;

        public float totalCost => realCost + heuristicCost;
    }
}