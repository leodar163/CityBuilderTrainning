using GridSystem;

namespace PathFinding
{
    public class PathNode
    {
        public CellData parent;
        
        public float realCost;
        public float heuristicCost;

        public float extraMovementCost = 0;
        public float movementCostCoefficient = 1;
        public float[] extraMovementCostsToNeighbours;
        public float[] movementCoefficientsToNeighbours;
        
        
        public float totalCost => realCost + heuristicCost;
    }
}