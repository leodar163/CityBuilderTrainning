using Format;
using GridSystem;
using Interactions;
using ToolTipSystem;
using UnityEngine;
using UnityEngine.Localization;

namespace BuildingSystem.Facilities
{
    public class FacilityPlacer : Utils.Singleton<FacilityPlacer>, IInteractor, ITooltipMessenger
    {
        public ITooltipMessenger tooltipMessengerSelf => this;
        
        public static FacilityType selectedFacility { get; private set; }
        public bool isActive { get; private set; }

        [SerializeField] private TooltipMessageUI _tooltipMessage;

        public TooltipMessageUI tooltipMessage => _tooltipMessage;

        [Header("Construction")] 
        [SerializeField] private float _maxConstructionForceInvestment = 10;

        [SerializeField] private ConstructionSite _constructionSiteTemplate;

        public static float maxConstructionForceInvestment => Instance._maxConstructionForceInvestment;
         
        [Header("Messages")] 
        [SerializeField] private LocalizedString _notEnoughPlaceException;

        [SerializeField] private LocalizedString _CanBePlace;
        [SerializeField] private LocalizedString _CantBePlace;

        public InteractionMode interactionMode => InteractionMode.FacilityPlacing;
        
        private static ToolTipMessage _toolTipMessage;

        private bool _oneFacilityAsBeenPlaced;

        private void Awake()
        {
            selectedFacility = null;
        }

        private void Update()
        {
            if (!selectedFacility || !isActive) return;

            if(_oneFacilityAsBeenPlaced && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                EndPlacement();
                Tooltip.Unsub(this);
                return;
            }

            if (GridManager.HoveredCell == null)
            {
                Tooltip.Unsub(this);
                return;
            }
            
            if (CanPlaceFacility(selectedFacility, GridManager.HoveredCell) 
                && Input.GetMouseButtonUp(0) 
                && TryPlaceNewFacility(selectedFacility, GridManager.HoveredCell)
                && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                EndPlacement();
            }
        }

        public static void SelectFacilityToPlace(FacilityType facilityTypeToSelect)
        {
            selectedFacility = facilityTypeToSelect;
            InteractionManager.SwitchInteractionMode(InteractionMode.FacilityPlacing);
        }

        private static bool CanPlaceFacility(FacilityType facilityType, CellData cell)
        {
            if (cell == null) return false;

            FacilityType facilityToTest;

            if (facilityType is ConstructionSite constructionSite)
            {
                facilityToTest = constructionSite.facilityToBuild;
            }
            else
            {
                facilityToTest = facilityType;
            }
            
            bool EnoughPlace = cell.freeFacilityPlacements > 0;

            string enoughPlaceFormat = !EnoughPlace
                ? $"<color=#{FormatManager.negativeColorHTML}>{Instance._notEnoughPlaceException.GetLocalizedString()}</color>"
                : "";
            
            bool canBePlaced = facilityToTest.CanBePlaced(cell, out string conditionsFormat) && EnoughPlace ;

            conditionsFormat = enoughPlaceFormat + '\n' + conditionsFormat;
            
            _toolTipMessage = new ToolTipMessage
            {
                title = canBePlaced ? Instance._CanBePlace.GetLocalizedString() : Instance._CantBePlace.GetLocalizedString(),
                message = conditionsFormat
            };

            GridManager.PaintCursor(canBePlaced ? Color.green : Color.red);
            
            Tooltip.Sub(Instance);
            
            return canBePlaced;
        }
        
        private static bool TryPlaceNewFacility(FacilityType facilityTypeToPlace, CellData cell)
        {

            FacilityType facilityTemplate = facilityTypeToPlace.constructionCost <= 0
                ? facilityTypeToPlace
                : Instance._constructionSiteTemplate;

            if (!Instantiate(facilityTemplate.gameObject).TryGetComponent(out FacilityType facility))
                return false;

            if (!cell.TryAddFacility(facility))
            {
                Destroy(facility);
                return false;
            }

            Instance._oneFacilityAsBeenPlaced = true;            
            
            if (facility is ConstructionSite site)
            {
                site.SetFacilityToBuild(facilityTypeToPlace);
            }

            return true;

        }

        private static void EndPlacement()
        {
            Instance.isActive = false;
            selectedFacility = null;
            Instance._oneFacilityAsBeenPlaced = false;
            GridManager.PaintCursor(Color.white);
        }

        public ToolTipMessage ToToolTipMessage()
        {
            return _toolTipMessage;
        }

        void ITooltipMessenger.UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
            
        }
        
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