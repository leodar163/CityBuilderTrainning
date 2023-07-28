using UnityEngine;

namespace BuildingSystem
{
    public class PlaceableObject : MonoBehaviour
    {
        public Vector3Int[] range = {Vector3Int.zero};
        public Vector3Int currentCoordinates { get; private set; }
        public bool isPlaced { get; private set; }

        public Vector3Int[] GetAbsoluteCellRange()
        {
            Vector3Int[] absoluteRangeCell = new Vector3Int[range.Length];
            for (int i = 0; i < range.Length; i++)
            {
                absoluteRangeCell[i] = currentCoordinates + range[i];
            }

            return absoluteRangeCell;
        }

        public void Place(Vector3Int coordinates)
        {
            currentCoordinates = coordinates;
            GridManager.Instance.BlockCells(GetAbsoluteCellRange());
            isPlaced = true;
        }
    }
}