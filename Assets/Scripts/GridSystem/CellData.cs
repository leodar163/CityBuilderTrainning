using System.Collections.Generic;
using System.Linq;
using BuildingSystem.Facilities;
using OptiCollections;
using PathFinding;
using ResourceSystem.Markets;
using TerrainSystem;
using UnityEngine;

namespace GridSystem
{
    public class CellData : IHeapComparable<CellData>
    {
        public Market market;
        
        public Vector3Int coordinates { get; }
        public Vector3 position { get; private set; }
        public CellData[] neighbours { get; private set;}
        public readonly PathNode pathNode = new ();
        public TerrainType terrain { get; private set; }

        #region FACILITIES_PROPERTIES

        public int maxFacilityCount = 9;
        
        public readonly List<FacilityType> facilities = new();
        public readonly List<IEconomicActor> economicActors = new();
        public int facilityCount => facilities.Count;
        public int freeFacilityPlacements => maxFacilityCount - facilities.Count; 

        #endregion
        
        public CellData(Vector3Int coordinates)
        {
            this.coordinates = coordinates;
            position = GridManager.GetCellCenter(coordinates);
        }
        
       public void FindNeighbours()
        {
            neighbours = GridManager.FindNeighbours(this);
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
            int distX = Mathf.Abs(target.coordinates.x - coordinates.x);
            int distY = Mathf.Abs(target.coordinates.y - coordinates.y);
            
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

        public void SetTerrain(TerrainType terrainToSet)
        {
            if (terrainToSet != null && terrainToSet == terrain) return;
            
            terrain?.OnRemovedFromCell(this);
            terrain = terrainToSet;
            terrain?.OnAddedToCell(this);
        }

        #region FACILITIES_METHODS

        public bool TryAddFacility(FacilityType facilityTypeToAdd)
        {
            if (facilities.Contains(facilityTypeToAdd) || freeFacilityPlacements <= 0) return false;
            
            facilities.Add(facilityTypeToAdd);
            
            facilityTypeToAdd.OnAddedToCell(this);

            if (facilityTypeToAdd is IEconomicActor actor)
                OnAddEconomicActor(actor);
            
            return true;
        }
        
        public void RemoveFacility(FacilityType facilityTypeToRemove)
        {
            if (facilities.Contains(facilityTypeToRemove))
            {
                facilities.Remove(facilityTypeToRemove);
                facilityTypeToRemove.OnRemovedFromCell(this);
                
                if (facilityTypeToRemove is IEconomicActor actor)
                    OnRemoveEconomicActor(actor);
            }
        }

        public FacilityType GetFacility(int index)
        {
            return index > facilities.Count ? null : facilities[index];
        }

        private void OnAddEconomicActor(IEconomicActor actor)
        {
            if (!economicActors.Contains(actor))
                economicActors.Add(actor);
        }

        private void OnRemoveEconomicActor(IEconomicActor actor)
        {
            if (economicActors.Contains(actor))
                economicActors.Remove(actor);
        }
        
        #endregion

        public void OnMonthUpdate()
        {
            
        }
    }
}