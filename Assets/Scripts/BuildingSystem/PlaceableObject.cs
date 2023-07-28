using UnityEngine;

namespace BuildingSystem
{
    public class PlaceableObject : MonoBehaviour
    {
        public Vector3Int[] range = {Vector3Int.zero};
        public Vector3Int currentCoordinates { get; private set; }
        public bool isPlaced { get; private set; }


        public void Place(Vector3Int coordinates)
        {
            currentCoordinates = coordinates;
            isPlaced = true;
        }
    }
}