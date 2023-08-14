using System;
using UnityEngine;
using UnityEngine.UI;

namespace BuildingSystem.Facilities.UI
{
    public class FacilityButton : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Button _button;
        [SerializeField] private Animator _animator;
        
        private static readonly int animSelectionProperty = Animator.StringToHash("IsSelected");

        public Facility facility { get; private set; }

        private void Start()
        {
            _button.onClick.AddListener(() => FacilityPlacer.SelectFacility(facility));
        }

        private void Update()
        {
            _button.interactable = !FacilityPlacer.selectedFacility;
            _animator.SetBool(animSelectionProperty, FacilityPlacer.selectedFacility == facility);
        }

        public void AssignFacility(Facility facilityToAssign)
        {
            facility = facilityToAssign;
            _iconImage.sprite = facility.icon;
        }
    }
}