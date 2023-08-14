using System;
using System.Collections.Generic;
using BuildingSystem.Facilities;
using GridSystem;
using ResourceSystem;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace TerrainSystem
{
    [Serializable]
    public abstract class TerrainData : MonoBehaviour, ICellModifier, IResourceUpdater
    {
        public Tile tile;
        private readonly List<Facility> facilities = new();
        public int maxFacilityCount = 10;
        public int freeFacilityPlacements => maxFacilityCount - facilities.Count; 
        public CellData cell { get; private set; }
        public ResourceDeck resourceDeck;

        public virtual void OnAddedToCell(CellData cell)
        {
            this.cell = cell;
            GridManager.PaintTilemap(tile,GridManager.TileMapType.Terrain, cell.cell);
            cell.AttachTerrain(this);
            transform.position = cell.position;
            foreach (var facility in facilities)
            {
                facility.OnAddedToCell(cell);
            }
        }

        public virtual void OnRemovedFromCell(CellData cell)
        {
            if (this.cell != cell) return;
            GridManager.PaintTilemap(null,GridManager.TileMapType.Terrain,cell.cell);
            cell.DetachTerrain();
            this.cell = null;
            foreach (var facility in facilities)
            {
                facility.OnRemovedFromCell(cell);
            }
        }

        public void OnRemoveFromCell()
        {
            if (cell != null)
            {
                OnRemovedFromCell(cell);
            }
        }

        public bool TryAddFacility(Facility facilityToAdd)
        {
            if (facilities.Contains(facilityToAdd)) return false;
            if (cell != null)
            {
                facilityToAdd.OnAddedToCell(cell);
            }
            facilityToAdd.transform.parent = transform;
            facilities.Add(facilityToAdd);
            PlaceFacility(facilityToAdd);
            
            return true;
        }

        private void PlaceFacility(Facility facilityToPlace)
        {
            Vector3 position = new Vector3
            {
                x = Random.Range(-0.5f, 0.5f),
                z = Random.Range(-0.5f, 0.5f)
            };

            facilityToPlace.transform.localPosition = position;
        }

        public virtual void OnUpdateResources(ResourceDeck resources)
        {
            foreach (var facility in facilities)
            {
                facility.OnUpdateResources(resources);
            }
        }
    }
}