using System;
using System.Collections.Generic;
using GridSystem;
using TerrainSystem;
using Unity.Mathematics;
using UnityEngine;

namespace BuildingSystem.Facilities.Behaviors
{
    public enum PlacementType
    {
        Random,
        Center,
        CostLine
    }
    
    public static class FacilityPlacementBehaviors
    {
        private static TerrainType s_costTerrain;
        
        public static void Place(FacilityType facility, CellData cell, PlacementType type)
        {
            switch (type)
            {
                case PlacementType.Center:
                    PlaceCenter(facility,  cell);
                    break;
                case PlacementType.Random:
                    PlaceRandom(facility, cell);
                    break;
                case PlacementType.CostLine:
                    PlaceCostLine(facility, cell);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static void PlaceCenter(FacilityType facility, CellData cell)
        {
            Vector3 facilityPosition = cell.position;
            Quaternion rotation = quaternion.Euler(new float3(0, UnityEngine.Random.Range(-175, 175),0));
            Vector3 scale = Vector3.one * (UnityEngine.Random.Range(0.9f, 1.1f) * facility.ScaleMultiplier);
            
            facility.RenderingSelf.SetTransform(facilityPosition, rotation, scale);
        }

        private static void PlaceRandom(FacilityType facility, CellData cell)
        {
            bool isPositionValid;

            Vector3 facilityPosition;
            Quaternion rotation = quaternion.Euler(new float3(0, UnityEngine.Random.Range(-175, 175),0));
            Vector3 scale = Vector3.one * (UnityEngine.Random.Range(0.9f, 1.1f) * facility.ScaleMultiplier);

            int safetyLoopCount = 100;
            
            do
            {
                facilityPosition = cell.position + new Vector3
                {
                    x = UnityEngine.Random.Range(-0.5f, 0.5f),
                    z = UnityEngine.Random.Range(-0.5f, 0.5f)
                };

                isPositionValid = true;
                
                foreach (var fac in cell.facilities)
                {
                    if (fac == facility) continue;
                    if (Vector3.Distance(facilityPosition, fac.RenderingSelf.Position)
                        < facility.sizeRadius + fac.sizeRadius)
                    {
                        isPositionValid = false;
                        break;
                    }
                }

                safetyLoopCount--;
                
            } while (!isPositionValid && safetyLoopCount > 0);

            facility.RenderingSelf.SetTransform(facilityPosition, rotation, scale);
        }

        private static void PlaceCostLine(FacilityType facility, CellData cell)
        { 
            s_costTerrain ??= TerrainSet.Default.GetTerrain("terrain_cost");
            
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
                    int alea = UnityEngine.Random.Range(0, crossNeighbours.Count);

                    CellData neighbour = crossNeighbours[alea];
                    crossNeighbours.RemoveAt(alea);
                    
                    if (neighbour.terrain == s_costTerrain)
                    {
                        target = neighbour;
                        break;
                    }

                }
                
                if (target == null)
                {
                    while (diagonalNeighbours.Count > 0)
                    {
                        int alea = UnityEngine.Random.Range(0, diagonalNeighbours.Count);

                        CellData neighbour = diagonalNeighbours[alea];
                        diagonalNeighbours.RemoveAt(alea);

                        if (neighbour.terrain == s_costTerrain)
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
                            newPos.x = UnityEngine.Random.Range(-0.5f, 0.5f) + cell.position.x;
                        if (freeZ)
                            newPos.z = UnityEngine.Random.Range(-0.5f, 0.5f) + cell.position.z;

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
            
        }
    }
}