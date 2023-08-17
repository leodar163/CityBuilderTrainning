using System;
using System.Collections.Generic;
using Utils.UI;
using UnityEngine;

namespace BuildingSystem.Facilities.UI
{
    public class FacilityBuildingPanelUI : PanelUI<FacilityBuildingPanelUI>
    {
        [Tooltip("If null, will be equal to 'FacilitySet.Default'")]
        public FacilitySet set;
        [SerializeField] private RectTransform _child;
        [SerializeField] private FacilityButton _facilityButtonTemplate;
        [SerializeField] private RectTransform _buttonLayout;
        private List<FacilityButton> _buttons = new ();
        

        protected override void Awake()
        {
            base.Awake();
            
            if (!set) set = FacilitySet.Default;
            
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (!set) return;

            foreach (var facility in set.facilities)
            {
                if (Instantiate(_facilityButtonTemplate.gameObject, _buttonLayout).TryGetComponent(out FacilityButton newButton))
                {
                    newButton.AssignFacility(facility);
                    _buttons.Add(newButton);
                }
            }
        }
        
        public override void OpenPanel()
        {
            base.OpenPanel();
            
            _child.gameObject.SetActive(true);
        }

        public override void ClosePanel()
        {
            base.ClosePanel();
            
            _child.gameObject.SetActive(false);
        }
    }
}