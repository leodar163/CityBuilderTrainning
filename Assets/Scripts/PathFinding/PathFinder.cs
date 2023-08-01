using System;
using System.Collections.Generic;
using GridSystem;
using UnityEngine;
using Utils;

namespace PathFinding
{
    public class PathFinder : Singleton<PathFinder>
    {
        [SerializeField] private Vector3Int origin;
        [SerializeField] private Vector3Int target;
        [SerializeField] private bool refreshPath;

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(GridManager.GetCellCenter(origin), 0.1f);
            Gizmos.DrawSphere(GridManager.GetCellCenter(target), 0.1f);
            
            if (refreshPath && Application.isPlaying && TryFindPath(origin, target, out CellData[] path))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(GridManager.GetCellCenter(path[0]), 0.2f);
                for (int i = 0; i < path.Length - 1; i++)
                {
                    Vector3 begin = GridManager.GetCellCenter(path[i]) + Vector3.up * 0.1f;
                    Vector3 end = GridManager.GetCellCenter(path[i + 1]) + Vector3.up * 0.1f;
                    
                    Gizmos.DrawLine(begin, end);
                    Gizmos.DrawSphere(end, 0.2f);
                }
            }
        }

        /// <summary>
        /// Use A* algorithm to find path bitween two cells
        /// </summary>
        /// <param name="currentCell"></param>
        /// <param name="targetCell"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool TryFindPath(Vector3Int currentCell, Vector3Int targetCell, out CellData[] path)
        {
            if (GridManager.CellIsOutOfGrid(currentCell) || GridManager.CellIsOutOfGrid(targetCell))
            {
                throw new ArgumentOutOfRangeException("You're trying to find a path out of the grid size. Don't.",
                    new Exception());
            }
            CellData origin = GridManager.GetCellData(currentCell);
            CellData target = GridManager.GetCellData(targetCell);

            List<CellData> openSet = new List<CellData>();
            HashSet<CellData> closedSet = new HashSet<CellData>();
            
            openSet.Add(origin);

            while (openSet.Count > 0)
            {
                CellData cursor = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].pathNode.totalCost < cursor.pathNode.totalCost
                        || openSet[i].pathNode.totalCost == cursor.pathNode.totalCost &&
                        openSet[i].pathNode.heuristicCost < cursor.pathNode.heuristicCost)
                    {
                        cursor = openSet[i];
                    }
                }

                openSet.Remove(cursor);
                closedSet.Add(cursor);

                if (cursor == target)
                {
                    print("path found");
                    path = RetracePath(origin, target);
                    return true;
                }

                foreach (var neighbour in cursor.neighbours)
                {
                    if (neighbour.isBlocked || closedSet.Contains(neighbour))
                        continue;

                    float movementCostToNeighbour = cursor.pathNode.realCost + cursor.DistanceFrom(neighbour);

                    //print($"{movementCostToNeighbour} < {neighbour.pathNode.realCost}");

                    if (movementCostToNeighbour < neighbour.pathNode.realCost || !openSet.Contains(neighbour))
                    {
                        neighbour.pathNode.realCost = movementCostToNeighbour;
                        neighbour.pathNode.heuristicCost = neighbour.DistanceFrom(target);
                        neighbour.pathNode.parent = cursor;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
            print("Path not found");
            path = Array.Empty<CellData>();
            return false;
        }

        private static CellData[] RetracePath(CellData origin, CellData target)
        {
            List<CellData> path = new List<CellData>();
            
            CellData cursor = target;
            path.Add(cursor);
            
            do
            {
                CellData parent = cursor.pathNode.parent;
                cursor = parent;
                path.Add(parent);
            } while (cursor != origin);

            path.Reverse();
            return path.ToArray();
        }
    }
}