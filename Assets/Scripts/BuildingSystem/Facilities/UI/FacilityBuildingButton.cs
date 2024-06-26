﻿using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BuildingSystem.Facilities.UI
{
    public class FacilityBuildingButton : MonoBehaviour, ITooltipMessenger, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Button _button;
        [SerializeField] private Animator _animator;
        
        
        private static readonly int animSelectionProperty = Animator.StringToHash("IsSelected");

        public FacilityType facility { get; private set; }
        
        [Header("Tooltip")]
        [SerializeField] private FacilityTooltipMessageUI _facilityTooltipMessageTemplate;
        public TooltipMessageUI tooltipMessage => _facilityTooltipMessageTemplate;
        public ITooltipMessenger tooltipMessengerSelf => this;
        
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
                facilityUI.SetFacility(facility);
            }
        }

        private void Start()
        {
            _button.onClick.AddListener(() => FacilityPlacer.SelectFacilityToPlace(facility));
        }

        private void Update()
        {
            _animator.SetBool(animSelectionProperty, FacilityPlacer.selectedFacility == facility);
        }

        public void AssignFacility(FacilityType facilityTypeToAssign)
        {
            facility = facilityTypeToAssign;
            
            _iconImage.sprite = facility.icon;
        }
    }
}