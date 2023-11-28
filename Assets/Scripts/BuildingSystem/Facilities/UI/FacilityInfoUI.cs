using TMPro;
using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BuildingSystem.Facilities.UI
{
    public class FacilityInfoUI : MonoBehaviour, ITooltipMessenger, IPointerEnterHandler, IPointerExitHandler
    {
        private FacilityType _facility;
        
        public FacilityType Facility
        {
            get => _facility;
            set
            {
                if (_facility != null)
                {
                    _deleteButton.onClick.RemoveAllListeners();
                }
                _facility = value;
                DisplayFacility();  
            } 
        }

        private ConstructionSite _constructionSite;

        [SerializeField] private Image _facilityIcon;
        [SerializeField] private Button _deleteButton;

        [Header("Tooltip")]
        [SerializeField] private FacilityTooltipMessageUI _facilityTooltipMessageTemplate;
        public TooltipMessageUI tooltipMessage => _facilityTooltipMessageTemplate;
        public ITooltipMessenger tooltipMessengerSelf => this;

        private void Awake()
        {
            if (_deleteButton)
                _deleteButton.onClick.AddListener(DestroyFacility);
        }

        private void DisplayFacility()
        {
            if (_facility == null) return;

            _constructionSite = _facility as ConstructionSite;

            DisplayFacility(_constructionSite == null ? _facility : _constructionSite.facilityToBuild);
        }

        private void DisplayFacility(FacilityType facility)
        {
            _facilityIcon.sprite = facility.icon;
        }

        private void DestroyFacility()
        {
            _facility.cell.RemoveFacility(_facility);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipMessengerSelf.SubToTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipMessengerSelf.UnsubFromTooltip();
        }

        
        public void UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
            if (messageUI is FacilityTooltipMessageUI facilityUI)
            {
                facilityUI.SetFacility(_constructionSite == null ? _facility : _constructionSite.facilityToBuild);
            }
        }
    }
}