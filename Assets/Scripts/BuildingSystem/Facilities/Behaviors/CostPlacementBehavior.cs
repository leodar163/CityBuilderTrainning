using System.Collections.Generic;
using GridSystem;
using TerrainSystem.Scriptables;
using UnityEngine;

namespace BuildingSystem.Facilities.Behaviors
{
    [CreateAssetMenu(menuName = "Facilities/Behaviors/Cost Placement", fileName = "CostPlacementBehavior")]
    public class CostPlacementBehavior : FacilityBehavior
    {
        [SerializeField] private ScriptableTerrain _costTerrain;
        
        public override void OnAddedToCell(FacilityType facility, CellData cell)
        {

            CellData target = null;
            Vector3 newPos = facility.RenderingSelf.Position;
            List<CellData> neighbours = new List<CellData>(cell.neighbours);

            List<CellData> crossNeighbours = new List<CellData>();
            List<CellData> diagonalNeighbours = new List<CellData>();

            foreach (var neighbour in neighbours)
            {
                if (neighbour.coordinates.x == cell.coordinates.x || neighbour.coordinates.y == cell.coordinates.y)
                {
                    crossNeighbours.Add(neighbour);
                }
                else
                {
                    diagonalNeighbours.Add(neighbour);
                }
            } 
            
            for (int i = 0; i < 8; i++)
            { 
                while (crossNeighbours.Count > 0)
                {
                    int alea = Random.Range(0, crossNeighbours.Count);

                    CellData neighbour = crossNeighbours[alea];
                    crossNeighbours.RemoveAt(alea);
                    
                    if (neighbour.terrain == _costTerrain.terrain)
                    {
                        target = neighbour;
                        break;
                    }

                }
                
                if (target == null)
                {
                    while (diagonalNeighbours.Count > 0)
                    {
                        int alea = Random.Range(0, diagonalNeighbours.Count);

                        CellData neighbour = diagonalNeighbours[alea];
                        diagonalNeighbours.RemoveAt(alea);

                        if (neighbour.terrain == _costTerrain.terrain)
                        {
                            target = neighbour;
                            break;
                        }

                    }
                }
            
                if (target == null) return;

                newPos = Vector3.Lerp(cell.position, target.position, 0.5f);
                
                bool freeX = cell.coordinates.x == target.coordinates.x;
                bool freeZ = cell.coordinates.y == target.coordinates.y;

                int safetyLoopCount = 100;
                bool isPositionValid = true;
                
                if (freeX || freeZ)
                {
                    do
                    {
                        if (freeX)
                            newPos.x = Random.Range(-0.5f, 0.5f) + cell.position.x;
                        if (freeZ)
                            newPos.z = Random.Range(-0.5f, 0.5f) + cell.position.z;

                        isPositionValid = true;

                        foreach (var fac in cell.facilities)
                        {
                            if (fac == facility) continue;
                            if (Vector3.Distance(newPos, fac.RenderingSelf.Position)
                                < facility.sizeRadius + fac.sizeRadius)
                            {
                                isPositionValid = false;
                                break;
                            }
                        }

                        safetyLoopCount--;

                    } while (!isPositionValid && safetyLoopCount > 0);
                }
                if (isPositionValid) break;
            }
           
            if (target == null) return;

            facility.RenderingSelf.SetTransform(newPos, Quaternion.LookRotation((target.position - cell.position).normalized), facility.RenderingSelf.Scale);
            
            //facility.TryRemoveBehavior(this);
        }
    }
}