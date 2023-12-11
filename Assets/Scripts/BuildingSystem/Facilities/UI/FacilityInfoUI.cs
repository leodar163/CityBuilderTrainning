using System;
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
                _facility = value;
                DisplayFacility();  
                onFacilityChanged?.Invoke(_facility);
            } 
        }

        private ConstructionSite _constructionSite;

        [SerializeField] private Image _facilityIcon;

        [Header("Tooltip")]
        [SerializeField] private FacilityTooltipMessageUI _facilityTooltipMessageTemplate;
        public TooltipMessageUI tooltipMessage => _facilityTooltipMessageTemplate;
        public ITooltipMessenger tooltipMessengerSelf => this;

        public event Action<FacilityType> onFacilityChanged;
        
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