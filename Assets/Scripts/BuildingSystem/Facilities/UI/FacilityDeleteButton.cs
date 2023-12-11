using System;
using Localization;
using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BuildingSystem.Facilities.UI
{
    public class FacilityDeleteButton : MonoBehaviour, ITooltipMessenger, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextTooltipMessageUI _tooltipMessage;
        [SerializeField] private FacilityInfoUI _facilityInfo;
        [SerializeField] private Button _button;

        private FacilityType _facility;

        public ITooltipMessenger tooltipMessengerSelf => this;
        public TooltipMessageUI tooltipMessage => _tooltipMessage;

        private static string destroyFacilityMessage = "";
        private static string cantDestroyMessage = "";

        private void Start()
        {
            if (destroyFacilityMessage == "")
            {
                destroyFacilityMessage = VariableNameManager.DestroyFacilityMessage.String;
                VariableNameManager.DestroyFacilityMessage.onStringChanged += s => destroyFacilityMessage = s.String;
            }
            if (cantDestroyMessage == "")
            {
                cantDestroyMessage = VariableNameManager.FacilityCantDestroy.String;
                VariableNameManager.FacilityCantDestroy.onStringChanged += s => cantDestroyMessage = s.String;
            }

            AssignFacility(_facilityInfo.Facility);
            _facilityInfo.onFacilityChanged += AssignFacility;
        }
        
        private void OnDestroy()
        {
            _facilityInfo.onFacilityChanged -= AssignFacility;
        }

        private void AssignFacility(FacilityType facility)
        {
            if (_facility != null && _facility == facility) return;

            _facility = facility;
            
            _button.onClick.RemoveAllListeners();
            
            if (_facility == null) return;
            
            _button.onClick.AddListener(() => _facility.cell.RemoveFacility(_facility));
            _button.interactable = _facility.destroyable;
        }
        
        public void UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
            if (messageUI is TextTooltipMessageUI textTooltipMessageUI)
            {
                textTooltipMessageUI.SetTexts(_facility.destroyable
                    ? destroyFacilityMessage
                    : cantDestroyMessage, "");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipMessengerSelf.SubToTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipMessengerSelf.UnsubFromTooltip();
        }
    }
}