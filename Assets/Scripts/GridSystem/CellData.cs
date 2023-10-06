using System.Collections.Generic;
using BuildingSystem;
using OptiCollections;
using PathFinding;
using ResourceSystem;
using ResourceSystem.Market;
using ResourceSystem.Scriptables;
using ResourceSystem.Transactions;
using TerrainSystem;
using UnityEngine;

namespace GridSystem
{
    public class CellData : IHeapComparable<CellData>, ITransactor
    {
        public Market market;
        
        public Vector3Int cellCoordinates { get; }
        public Vector3 position { get; private set; }
        public CellData[] neighbours { get; private set;}
        public readonly PathNode pathNode = new ();
        public TerrainType terrain { get; private set; }
        
        
        List<ResourceContainer> ITransactor.registry { get; } = new();
        public ITransactor transactorSelf => this;
        
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

        public void AttachTerrain(TerrainType terrainType)
        {
            terrain = terrainType;
            terrain.transform.position = position;
/*
            transactorSelf.RemoveContainersAll();
            
            transactorSelf.InitContainers(terrain.deckTemplate == null 
                ? ResourceDeck.Default 
              : terrain.deckTemplate);
              */
        }

        public void DetachTerrain()
        {
            terrain = null;
        }

        public void OnMonthUpdate()
        {
            transactorSelf.AskInputs();
            transactorSelf.GiveOutputs();
        }
    }
}