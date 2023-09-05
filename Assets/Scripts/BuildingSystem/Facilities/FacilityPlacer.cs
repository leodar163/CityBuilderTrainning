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
        public static FacilityType SelectedFacilityType { get; private set; }

        [Header("Messages")] 
        [SerializeField] private LocalizedString _notEnoughPlaceException;

        [SerializeField] private LocalizedString _CanBePlace;
        [SerializeField] private LocalizedString _CantBePlace;
        
        private static ToolTipMessage _toolTipMessage;

        private bool _oneFacilityAsBeenPlaced;

        private void Awake()
        {
            SelectedFacilityType = null;
        }

        private void Update()
        {
            if (!SelectedFacilityType || !isActive) return;

            if(_oneFacilityAsBeenPlaced && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                EndPlacement();
                return;
            }
            
            if (CanPlaceFacility(SelectedFacilityType, GridManager.HoveredCell) 
                && Input.GetMouseButtonUp(0) 
                && TryPlaceNewFacility(SelectedFacilityType, GridManager.HoveredCell)
                && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                EndPlacement();
            }
        }

        public static void SelectFacility(FacilityType facilityTypeToSelect)
        {
            SelectedFacilityType = facilityTypeToSelect;
            InteractionManager.SwitchInteractionMode(InteractionMode.FacilityPlacing);
        }

        private static bool CanPlaceFacility(FacilityType facilityType, CellData cell)
        {
            if (cell == null) return false;
            
            TerrainType terrain = cell.terrain;
            
            bool notEnoughPlace = terrain.freeFacilityPlacements > 0;
            
            string conditionsFormat = notEnoughPlace ? Instance._notEnoughPlaceException.GetLocalizedString() : "";
            
            bool canBePlaced = notEnoughPlace && facilityType.CanBePlaced(terrain, out conditionsFormat);

            _toolTipMessage = new ToolTipMessage
            {
                title = canBePlaced ? Instance._CanBePlace.GetLocalizedString() : Instance._CantBePlace.GetLocalizedString(),
                message = conditionsFormat
            };
            ToolTip.Sub(Instance);
            
            GridManager.PaintCursor(canBePlaced ? Color.green : Color.red);
            
            return canBePlaced;
        }
        
        private static bool TryPlaceNewFacility(FacilityType facilityTypeToPlace, CellData cell)
        {
            if (Instantiate(facilityTypeToPlace.gameObject).TryGetComponent(out FacilityType newFacility) 
                && cell.terrain.TryAddFacility(newFacility))
            {
                Instance._oneFacilityAsBeenPlaced = true;
                return true;
            }

            Destroy(newFacility.gameObject);
            
            return false;
        }

        private static void EndPlacement()
        {
            Instance.isActive = false;
            SelectedFacilityType = null;
            Instance._oneFacilityAsBeenPlaced = false;
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