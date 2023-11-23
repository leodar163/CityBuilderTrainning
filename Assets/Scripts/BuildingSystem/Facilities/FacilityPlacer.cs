using BuildingSystem.Facilities.Scriptables;
using Format;
using GridSystem;
using GridSystem.Interaction;
using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using UnityEngine.Localization;

namespace BuildingSystem.Facilities
{
    public class FacilityPlacer : Utils.Singleton<FacilityPlacer>, IGridInteractor, ITooltipMessenger
    {
        public ITooltipMessenger tooltipMessengerSelf => this;
        
        public static FacilityType selectedFacility { get; private set; }
        public GridInteractorType type => GridInteractorType.FacilityPlacer;
        public bool isActive { get; set; }
        public bool cancelable => true;

        public void OnHoveredCellChanged(CellData cellData)
        {
            if (cellData == null)
            {
                tooltipMessengerSelf.UnsubFromTooltip();
            }
            else
            {
                tooltipMessengerSelf.SubToTooltip();
            }
        }

        void IGridInteractor.OnActivated()
        {
            
        }

        void IGridInteractor.OnDeactivated()
        {
           EndPlacement();
        }

        [SerializeField] private TextTooltipMessageUI _textTooltipMessage;
        private string _placementFeedbackFormat;
        private string _placementConditionFormat;

        public TooltipMessageUI tooltipMessage => _textTooltipMessage;

        [Header("Construction")] 
        [SerializeField] private float _maxConstructionForceInvestment = 10;

        [SerializeField] private ScriptableConstructionSite _constructionSiteTemplate;

        public static float maxConstructionForceInvestment => Instance._maxConstructionForceInvestment;
         
        [Header("Messages")] 
        [SerializeField] private LocalizedString _notEnoughPlaceException;

        [SerializeField] private LocalizedString _CanBePlace;
        [SerializeField] private LocalizedString _CantBePlace;

        private bool _oneFacilityAsBeenPlaced;

        private void Awake()
        {
            selectedFacility = null;
        }

        private void Update()
        {
            if (selectedFacility == null || !isActive) return;

            if(_oneFacilityAsBeenPlaced && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                EndPlacement();
                return;
            }

            if (CanPlaceFacility(selectedFacility, GridEventSystem.HoveredCell) 
                && Input.GetMouseButtonUp(0) 
                && TryPlaceNewFacility(selectedFacility, GridEventSystem.HoveredCell)
                && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                EndPlacement();
            }
        }

        public static void SelectFacilityToPlace(FacilityType facilityTypeToSelect)
        {
            selectedFacility = facilityTypeToSelect;
            GridEventSystem.SwitchInteractor(GridInteractorType.FacilityPlacer);
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
            
            GridEventSystem.PaintCursor(canBePlaced ? Color.green : Color.red);
            
            Instance.tooltipMessengerSelf.SubToTooltip();

            return canBePlaced;
        }
        
        private static bool TryPlaceNewFacility(FacilityType facilityTypeToPlace, CellData cell)
        {
            
            FacilityType facility = facilityTypeToPlace.constructionCost > 0
                ? Instance._constructionSiteTemplate.GetFacilityCopy()
                : facilityTypeToPlace.Copy();

            if (facility is ConstructionSite constructionSite)
            {
                constructionSite.SetFacilityToBuild(facilityTypeToPlace);
            }

            if (!cell.TryAddFacility(facility))
            {
                facility.RenderingSelf.OnDestroyed();
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
            Instance.tooltipMessengerSelf.UnsubFromTooltip();
            Instance.isActive = false;
            selectedFacility = null;
            Instance._oneFacilityAsBeenPlaced = false;
            GridEventSystem.PaintCursor(Color.white);
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