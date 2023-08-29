using System;
using GridSystem;
using Interactions;
using TerrainSystem;
using ToolTipSystem;
using UnityEngine;
using UnityEngine.Localization;
using Utils;

namespace BuildingSystem.Facilities
{
    public class FacilityPlacer : Singleton<FacilityPlacer>, IToolTipSpeaker, IInteractionMode
    {
        public static Facility selectedFacility { get; private set; }

        [Header("Messages")] 
        [SerializeField] private LocalizedString _notEnoughPlaceException;

        [SerializeField] private LocalizedString _CanBePlace;
        [SerializeField] private LocalizedString _CantBePlace;
        
        private static ToolTipMessage _toolTipMessage;

        private void Awake()
        {
            selectedFacility = null;
        }

        private void Update()
        {
            if (!selectedFacility || !isActive) return;

            if (CanPlaceFacility(selectedFacility, GridManager.HoveredCell) 
                && Input.GetMouseButtonUp(0) 
                && TryPlaceNewFacility(selectedFacility, GridManager.HoveredCell)
                && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                EndPlacement();   
            }
        }

        public static void SelectFacility(Facility facilityToSelect)
        {
            selectedFacility = facilityToSelect;
            InteractionManager.SwitchInteractionMode(InteractionMode.FacilityPlacing);
        }

        private static bool CanPlaceFacility(Facility facility, CellData cell)
        {
            if (cell == null) return false;
            
            TerrainType terrain = cell.terrain;
            
            bool notEnoughPlace = terrain.freeFacilityPlacements > 0;
            
            string conditionsFormat = notEnoughPlace ? Instance._notEnoughPlaceException.GetLocalizedString() : "";
            
            bool canBePlaced = notEnoughPlace && facility.CanBePlaced(terrain, out conditionsFormat);

            _toolTipMessage = new ToolTipMessage
            {
                title = canBePlaced ? Instance._CanBePlace.GetLocalizedString() : Instance._CantBePlace.GetLocalizedString(),
                message = conditionsFormat
            };
            ToolTip.Sub(Instance);
            
            GridManager.PaintCursor(canBePlaced ? Color.green : Color.red);
            
            return canBePlaced;
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
            Instance.isActive = false;
            selectedFacility = null;
            GridManager.PaintCursor(Color.white);
        }

        public ToolTipMessage ToToolTipMessage()
        {
            return _toolTipMessage;
        }

        public bool isActive { get; private set; }
        
        public void ActivateMode()
        {
            isActive = true;
        }

        public void DeactivateMode()
        {
            EndPlacement();
        }
    }
}