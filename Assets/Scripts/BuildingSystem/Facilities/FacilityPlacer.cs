using System;
using GridSystem;
using UnityEngine;
using Utils;
using TerrainData = TerrainSystem.TerrainData;

namespace BuildingSystem.Facilities
{
    public class FacilityPlacer : Singleton<FacilityPlacer>
    {
        public static Facility selectedFacility { get; private set; }

        private void Awake()
        {
            selectedFacility = null;
        }

        private void Update()
        {
            if (!selectedFacility) return;

            if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Escape))
            {
                EndPlacement(); 
                return;
            }

            if (CanPlaceFacility(selectedFacility, GridManager.HoveredCell) 
                && Input.GetMouseButtonUp(0) 
                && TryPlaceNewFacility(selectedFacility, GridManager.HoveredCell))
            {
                EndPlacement();   
            }
        }

        public static void SelectFacility(Facility facilityToSelect)
        {
            selectedFacility = facilityToSelect;
        }

        private static bool CanPlaceFacility(Facility facility, CellData cell)
        {
            if (cell == null) return false;
            
            TerrainData terrain = cell.terrain;

            bool canPlace = terrain.freeFacilityPlacements > 0 && facility.CanBePlaced(terrain); 
            
            GridManager.PaintCursor(canPlace ? Color.green : Color.red);
            
            return canPlace;
        }
        
        private static bool TryPlaceNewFacility(Facility facilityToPlace, CellData cell)
        {
            if (Instantiate(facilityToPlace.gameObject).TryGetComponent(out Facility newFacility) 
                && cell.terrain.TryAddFacility(newFacility))
            {
                return true;
            }

            Destroy(newFacility.gameObject);
            
            return false;
        }

        private static void EndPlacement()
        {
            selectedFacility = null;
            GridManager.PaintCursor(Color.white);
        }
    }
}