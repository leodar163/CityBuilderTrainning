using System;
using System.Collections.Generic;
using Utils.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace BuildingSystem.Facilities.UI
{
    public class FacilityBuildingPanelUI : PanelUI<FacilityBuildingPanelUI>
    {
        [FormerlySerializedAs("set")] [Tooltip("If null, will be equal to 'FacilitySet.Default'")]
        public FacilitySet facilitySet;
        [SerializeField] private RectTransform _child;
        [FormerlySerializedAs("_facilityButtonTemplate")] [SerializeField] private FacilityBuildingButton facilityBuildingButtonTemplate;
        [SerializeField] private RectTransform _buttonLayout;
        private List<FacilityBuildingButton> _buttons = new ();
        

        protected override void Awake()
        {
            base.Awake();
            
            if (!facilitySet) facilitySet = FacilitySet.Default;
            
            InitUI();
        }

        private void Update()
        {
            if(!isOpen) return;
        }

        private void InitUI()
        {
            if (!facilitySet) return;

            foreach (var facility in facilitySet.facilities)
            {
                if (Instantiate(facilityBuildingButtonTemplate.gameObject, _buttonLayout).TryGetComponent(out FacilityBuildingButton newButton))
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