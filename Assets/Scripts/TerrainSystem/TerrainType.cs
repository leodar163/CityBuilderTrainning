using System;
using System.Collections.Generic;
using BuildingSystem.Facilities;
using GridSystem;
using ResourceSystem;
using ToolTipSystem;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace TerrainSystem
{
    [Serializable]
    public abstract class TerrainType : MonoBehaviour, ICellModifier, IToolTipSpeaker, IResourceModifier
    {
        [SerializeField] private LocalizedString _terrainName;
        public Tile tile;
        private readonly List<Facility> _facilities = new();
        public int maxFacilityCount = 10;
        public int facilityCount => _facilities.Count;
        public int freeFacilityPlacements => maxFacilityCount - _facilities.Count; 
        public CellData cell { get; private set; }
        [SerializeField] private ScriptableResourceDeck _resourceDeckTemplate;
        public ResourceDeck resourceDeck { get; private set; }

        [Header("Description")] 
        [SerializeField] private string _typeDescription = "This is a terrain";
        [SerializeField] private string _effectDescription = "No effect";
        
        public string terrainName => _terrainName.GetLocalizedString();
        public string modifierName => terrainName;

        protected void Awake()
        {
            if (_resourceDeckTemplate == null) _resourceDeckTemplate = ScriptableResourceDeck.Default;
            resourceDeck = _resourceDeckTemplate.GetResourceDeckCopy();
            resourceDeck.Sub(this);
        }

        public virtual void OnAddedToCell(CellData cell)
        {
            this.cell = cell;
            GridManager.PaintTilemap(tile,GridManager.TileMapType.Terrain, cell.cellCoordinates);
            cell.AttachTerrain(this);
            transform.position = cell.position;
            foreach (var facility in _facilities)
            {
                facility.OnAddedToCell(cell);
            }
        }

        public virtual void OnRemovedFromCell(CellData cell)
        {
            if (this.cell != cell) return;
            GridManager.PaintTilemap(null,GridManager.TileMapType.Terrain,cell.cellCoordinates);
            cell.DetachTerrain();
            this.cell = null;
            foreach (var facility in _facilities)
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
            if (_facilities.Contains(facilityToAdd)) return false;
            if (cell != null)
            {
                facilityToAdd.OnAddedToCell(cell);
            }
            facilityToAdd.transform.parent = transform;
            _facilities.Add(facilityToAdd);
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
            facilityToPlace.OnAddedToCell(cell);
        }

        public void RemoveFacility(Facility facilityToRemove)
        {
            if (_facilities.Contains(facilityToRemove))
            {
                _facilities.Remove(facilityToRemove);
                facilityToRemove.OnRemovedFromCell(cell);
            }
        }

        public Facility GetFacility(int index)
        {
            return index > _facilities.Count ? null : _facilities[index];
        }


        public ToolTipMessage ToToolTipMessage()
        {
            return new ToolTipMessage
            {
                title = terrainName,
                message = $"{_typeDescription}\n\n{_effectDescription}"
            };
        }
        
        public virtual List<ResourceDelta> GetResourceDelta()
        {
            return null;
        }

        public List<ResourceDelta> GetPermanentResourceDelta()
        {
            return null;
        }
    }
}