using System.Collections.Generic;
using Utils.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace BuildingSystem.Facilities.UI
{
    public class FacilityBuildingPanelUI : PanelUI<FacilityBuildingPanelUI>
    {
        [Tooltip("If none, will be equal to 'FacilitySet.Default'")]
        public FacilitySet facilitySet;
        [SerializeField] private RectTransform _child;
        [FormerlySerializedAs("_facilityButtonTemplate")] [SerializeField] private FacilityBuildingButton facilityBuildingButtonTemplate;
        [SerializeField] private RectTransform _buttonLayout;
        private List<FacilityBuildingButton> _buttons = new ();
        

        private void Awake()
        {
            if (!facilitySet) facilitySet = FacilitySet.Default;
            
            InitUI();
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
    }
}