using BuildingSystem;
using PathFinding;
using Unity.VisualScripting;
using UnityEngine;

namespace GridSystem
{
    public class CellData
    {
        public PlaceableObject placedObject { get; private set; }
        public bool isBlocked => placedObject != null && placedObject.isPlaced;
        public Vector3Int cell;
        public CellData[] neighbours { get; private set;}
        public readonly PathNode pathNode = new ();
            
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
    }
}