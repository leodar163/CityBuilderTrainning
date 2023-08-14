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
            
            
            if (CanPlaceFacility(selectedFacility, GridManager.HoveredCell))
            {
                if (Input.GetMouseButtonUp(0))
                {
                    PlaceNewFacility(selectedFacility, GridManager.HoveredCell);
                    selectedFacility = null;
                    GridManager.PaintCursor(Color.white);
                }
            }
        }

        public static void SelectFacility(Facility facilityToSelect)
        {
            selectedFacility = facilityToSelect;
        }

        public static bool CanPlaceFacility(Facility facility, CellData cell)
        {
            TerrainData terrain = cell.terrain;

            bool canPlace = terrain.freeFacilityPlacements > 0 && facility.CanBePlaced(terrain); 
            
            GridManager.PaintCursor(canPlace ? Color.green : Color.red);
            
            return canPlace;
        }
        
        private static void PlaceNewFacility(Facility facilityToPlace, CellData cell)
        {
            if (Instantiate(facilityToPlace.gameObject).TryGetComponent(out Facility newFacility))
            {
                cell.terrain.AddFacility(newFacility);
            }
        }
    }
}