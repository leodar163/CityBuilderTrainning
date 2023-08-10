using System;
using System.Collections;
using System.Collections.Generic;
using GridSystem;
using OptiCollections;
using UnityEngine;
using Utils;

namespace PathFinding
{
    public class PathFinder : Singleton<PathFinder>
    {
        [SerializeField] private Vector3Int origin;
        [SerializeField] private Vector3Int target;

        private readonly Heap<CellData> _openSet = new ();
        private readonly HashSet<CellData> _closedSet = new ();

        private readonly Queue<PathRequest> _pathRequests = new ();
        
        private static Heap<CellData> s_openSet => Instance._openSet;
        private static HashSet<CellData> s_closedSet => Instance._closedSet;

        private static Queue<PathRequest> s_pathRequests => Instance._pathRequests;


        private IEnumerator _pathFindingRoutine;

        private CellData[] _feedBackPath = Array.Empty<CellData>();

        private void OnDrawGizmos()
        {
            if (_feedBackPath.Length > 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_feedBackPath[0].position, 0.2f);
                for (int i = 0; i < _feedBackPath.Length - 1; i++)
                {   
                    Vector3 begin = _feedBackPath[i].position + Vector3.up * 0.1f;
                    Vector3 end = _feedBackPath[i + 1].position + Vector3.up * 0.1f;
                    
                    Gizmos.DrawLine(begin, end);
                    /*if(i < _feedBackPath.Length - 2)*/ Gizmos.DrawSphere(end, 0.2f);
                }
            }
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(GridManager.GetCellCenter(origin) + Vector3.up * 0.1f, 0.1f);
            Gizmos.DrawSphere(GridManager.GetCellCenter(target) + Vector3.up * 0.1f, 0.1f);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.P))
            {
                RequestPathFinding(new PathRequest(origin, target,
                    delegate(bool b, CellData[] path)
                    {
                        _feedBackPath = path;
                    }
                ));
            }
        }

        public static void RequestPathFinding(PathRequest pathRequest)
        {
            s_pathRequests.Enqueue(pathRequest);

            if (Instance._pathFindingRoutine == null)
            {
                Instance._pathFindingRoutine = Instance.PathFindingRoutine();
                Instance.StartCoroutine(Instance._pathFindingRoutine);
            }
        }

        private IEnumerator PathFindingRoutine()
        {
            while (s_pathRequests.Count > 0)
            {
                
                PathRequest request = _pathRequests.Dequeue();
                bool pathFound = TryFindPath(request.origin, request.target, out CellData[] path);
                request.onFinished.Invoke(pathFound, path);
                
                yield return null;
            }

            _pathFindingRoutine = null;
        }
        
        /// <summary>
        /// Use A* algorithm to find path between two cells
        /// </summary>
        /// <returns>returns false if no path has been found</returns>
        private static bool TryFindPath(Vector3Int currentCell, Vector3Int targetCell, out CellData[] path)
        {
            if (GridManager.CellIsOutOfGrid(currentCell) || GridManager.CellIsOutOfGrid(targetCell))
            {
                throw new ArgumentOutOfRangeException("You're trying to find a path out of the grid size. Don't.",
                    new Exception());
            }
            CellData origin = GridManager.GetCellDataFromCellId(currentCell);
            CellData target = GridManager.GetCellDataFromCellId(targetCell);
            
            s_openSet.Clear();
            s_closedSet.Clear();
            
            s_openSet.Add(origin);

            while (s_openSet.Count > 0)
            {
                CellData cursor = s_openSet.GetAndRemoveFirstItem();
                s_closedSet.Add(cursor);

                if (cursor == target)
                {
                    path = RetracePath(origin, target);
                    return true;
                }

                for (int i = 0; i < cursor.neighbours.Length; i++)
                {
                    CellData neighbour = cursor.neighbours[i];
                    PathNode neighbourPathNode = neighbour.pathNode;
                    
                    if (neighbour.isBlocked || s_closedSet.Contains(neighbour))
                        continue;

                    
                    
                    float movementCostToNeighbour = 
                        cursor.pathNode.realCost + (cursor.DistanceFrom(neighbour) + 
                        neighbourPathNode.extraMovementCost + neighbourPathNode.extraMovementCostsToNeighbours[i]) * 
                        (neighbourPathNode.movementCostCoefficient + neighbourPathNode.movementCoefficientsToNeighbours[i]);

                    if (movementCostToNeighbour < neighbourPathNode.realCost || !s_openSet.Contains(neighbour))
                    {
                        neighbourPathNode.realCost = movementCostToNeighbour;
                        neighbourPathNode.heuristicCost = neighbour.DistanceFrom(target);
                        neighbourPathNode.parent = cursor;

                        if (!s_openSet.Contains(neighbour))
                        {
                            s_openSet.Add(neighbour);
                        }
                        else
                        {
                            s_openSet.UpdateItemPlace(neighbour);
                        }
                    }
                }
            }
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

    public struct PathRequest
    {
        public Vector3Int origin;
        public Vector3Int target;
        public readonly Action<bool, CellData[]> onFinished;

        public PathRequest(Vector3Int origin, Vector3Int target, Action<bool, CellData[]> onFinished)
        {
            this.origin = origin;
            this.target = target;
            this.onFinished = onFinished;
        }
        
        
    }
}