using BuildingSystem;
using OptiCollections;
using PathFinding;
using ResourceSystem;
using UnityEngine;
using TerrainData = TerrainSystem.TerrainData;

namespace GridSystem
{
    public class CellData : IHeapComparable<CellData>
    {
        public PlaceableObject placedObject { get; private set; }
        public bool isBlocked => placedObject != null && placedObject.isPlaced;
        public Vector3Int cell { get; }
        public Vector3 position { get; private set; }
        public CellData[] neighbours { get; private set;}
        public readonly PathNode pathNode = new ();
        public TerrainData terrain { get; private set; }

        public CellData(Vector3Int cell)
        {
            this.cell = cell;
            position = GridManager.GetCellCenter(cell);
        }
        
        public void PlaceObjectOn(PlaceableObject objectToPlace)
        {
            placedObject = objectToPlace;
        }

        public void RemoveObject()
        {
            placedObject = null;
        }

        public void FindNeighbours()
        {
            neighbours = GridManager.findNeighbours(this);
            pathNode.movementCoefficientsToNeighbours = new float[neighbours.Length];
            pathNode.extraMovementCostsToNeighbours = new float[neighbours.Length];
            
            for (int i = 0; i < neighbours.Length; i++)
            {
                pathNode.extraMovementCostsToNeighbours[i] = 0;
                pathNode.movementCoefficientsToNeighbours[i] = 1;
            }
        }

        public float DistanceFrom(CellData target)
        {
            int distX = Mathf.Abs(target.cell.x - cell.x);
            int distY = Mathf.Abs(target.cell.y - cell.y);
            
            return distX > distY ? 
                1.4f * distY + (distX - distY) 
                : 1.4f * distX + (distY - distX);
        }

        public static float Distance(CellData a, CellData b)
        {
            return a.DistanceFrom(b);
        }

        public float GetPriorityDifferenceWith(CellData other)
        {
            float difference = other.pathNode.totalCost - pathNode.totalCost;
            if (difference == 0)
                difference = other.pathNode.heuristicCost - pathNode.heuristicCost;
            return difference;
        }

        public void AttachTerrain(TerrainData terrainData)
        {
            terrain = terrainData;
        }

        public void DetachTerrain()
        {
            terrain = null;
        }
    }
}