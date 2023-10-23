using Format;
using GridSystem;
using Interactions;
using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using UnityEngine.Localization;

namespace BuildingSystem.Facilities
{
    public class FacilityPlacer : Utils.Singleton<FacilityPlacer>, IInteractor, ITooltipMessenger
    {
        public ITooltipMessenger tooltipMessengerSelf => this;
        
        public static FacilityType selectedFacility { get; private set; }
        public bool isActive { get; private set; }

        [SerializeField] private TextTooltipMessageUI _textTooltipMessage;
        private string _placementFeedbackFormat;
        private string _placementConditionFormat;

        public TooltipMessageUI tooltipMessage => _textTooltipMessage;

        [Header("Construction")] 
        [SerializeField] private float _maxConstructionForceInvestment = 10;

        [SerializeField] private ConstructionSite _constructionSiteTemplate;

        public static float maxConstructionForceInvestment => Instance._maxConstructionForceInvestment;
         
        [Header("Messages")] 
        [SerializeField] private LocalizedString _notEnoughPlaceException;

        [SerializeField] private LocalizedString _CanBePlace;
        [SerializeField] private LocalizedString _CantBePlace;

        public InteractionMode interactionMode => InteractionMode.FacilityPlacing;

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
                tooltipMessengerSelf.UnsubFromTooltip();
                return;
            }

            if (GridManager.HoveredCell == null)
            {
                tooltipMessengerSelf.UnsubFromTooltip();
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
            
            bool canBePlaced = facilityToTest.CanBePlaced(cell, out Instance._placementConditionFormat) && EnoughPlace ;

            Instance._placementConditionFormat = enoughPlaceFormat + '\n' + Instance._placementConditionFormat;
            Instance._placementFeedbackFormat = canBePlaced
                ? $"<color=#{FormatManager.positiveColorHTML}>{Instance._CanBePlace.GetLocalizedString()}"
                : $"<color=#{FormatManager.negativeColorHTML}>{Instance._CantBePlace.GetLocalizedString()}";
            
            GridManager.PaintCursor(canBePlaced ? Color.green : Color.red);
            
            Instance.tooltipMessengerSelf.SubToTooltip();

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

        void ITooltipMessenger.UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
            if (messageUI is TextTooltipMessageUI textTooltipMessageUI)
            {
                textTooltipMessageUI.SetTexts(_placementFeedbackFormat, _placementConditionFormat);
            }
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