using UnityEngine;

namespace BuildingSystem
{
    public class PlaceableObject : MonoBehaviour
    {
        public Vector3Int[] range = {Vector3Int.zero};
        public Vector3Int currentCoordinates { get; private set; }
        public bool isPlaced { get; private set; }
        
        public int rotationState { get; private set; }

        public void Rotate(int iterations = 1, bool clockwise = true)
        {
            transform.Rotate(Vector3.up, 90 * iterations * (clockwise ? 1 : -1));
            rotationState = (rotationState + iterations) % 4;
        }

        public Vector3Int[] GetAbsoluteCellRange()
        {
            return GetAbsoluteCellRange(currentCoordinates);
        }

        public Vector3Int[] GetAbsoluteCellRange(Vector3Int referenceCell)
        {
            Vector3Int[] absoluteRangeCell = new Vector3Int[range.Length];
            for (int i = 0; i < range.Length; i++)
            {
                Vector3Int rotatedCell = range[i];

                if (rotationState != 0)
                {
                    rotatedCell.x = rotationState switch
                    {
                        1 => range[i].y,
                        2 => -range[i].x,
                        3 => -range[i].y,
                        _ => rotatedCell.x
                    };
                    rotatedCell.y = rotationState switch
                    {
                        1 => -range[i].x,
                        2 => -range[i].y,
                        3 => range[i].x,
                        _ => rotatedCell.y
                    };
                }
                
                absoluteRangeCell[i] = rotatedCell + referenceCell;
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