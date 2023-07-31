using BuildingSystem;
using UnityEngine;

namespace GridSystem
{
    public struct CellData
    {
        public PlaceableObject placedObject { get; private set; }
        public bool isBlocked => placedObject != null && placedObject.isPlaced;
        public Vector3Int cell;
        
        public void PlaceObjectOn(PlaceableObject objectToPlace)
        {
            placedObject = objectToPlace;
        }

        public void RemoveObject()
        {
            placedObject = null;
        }
    }
}