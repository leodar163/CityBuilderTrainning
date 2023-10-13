﻿using System.Collections.Generic;
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
        
        public Vector3Int cellCoordinates { get; }
        public Vector3 position { get; private set; }
        public CellData[] neighbours { get; private set;}
        public readonly PathNode pathNode = new ();
        public TerrainType terrain { get; private set; }

        #region FACILITIES_PROPERTIES

        public int maxFacilityCount = 9;
        
        private readonly List<FacilityType> _facilities = new();
        public int facilityCount => _facilities.Count;
        public int freeFacilityPlacements => maxFacilityCount - _facilities.Count; 

        #endregion
        
        public CellData(Vector3Int cellCoordinates)
        {
            this.cellCoordinates = cellCoordinates;
            position = GridManager.GetCellCenter(cellCoordinates);
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
            int distX = Mathf.Abs(target.cellCoordinates.x - cellCoordinates.x);
            int distY = Mathf.Abs(target.cellCoordinates.y - cellCoordinates.y);
            
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
            
            terrain?.OnRemovedFromCell();
            terrain = terrainToSet;
            terrain?.OnAddedToCell(this);
        }

        #region FACILITIES_METHODS

        public bool TryAddFacility(FacilityType facilityTypeToAdd)
        {
            if (_facilities.Contains(facilityTypeToAdd)) return false;
            
            facilityTypeToAdd.OnAddedToCell(this);
            
            _facilities.Add(facilityTypeToAdd);
            PlaceFacility(facilityTypeToAdd);
            
            return true;
        }

        private void PlaceFacility(FacilityType facilityTypeToPlace)
        {
            Vector3 position = new Vector3
            {
                x = Random.Range(-0.5f, 0.5f),
                z = Random.Range(-0.5f, 0.5f)
            };

            facilityTypeToPlace.transform.localPosition = position;
            facilityTypeToPlace.OnAddedToCell(this);
        }

        public void RemoveFacility(FacilityType facilityTypeToRemove)
        {
            if (_facilities.Contains(facilityTypeToRemove))
            {
                _facilities.Remove(facilityTypeToRemove);
                facilityTypeToRemove.OnRemovedFromCell(this);
            }
        }

        public FacilityType GetFacility(int index)
        {
            return index > _facilities.Count ? null : _facilities[index];
        }

        #endregion

        public void OnMonthUpdate()
        {
            
        }
    }
}