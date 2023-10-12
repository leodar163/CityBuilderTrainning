using System;
using ToolTipSystem;
using UnityEngine;
using UnityEngine.UI;

namespace BuildingSystem.Facilities.UI
{
    public class FacilityButton : MonoBehaviour, IToolTipSpeaker
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Button _button;
        [SerializeField] private Animator _animator;
        
        private static readonly int animSelectionProperty = Animator.StringToHash("IsSelected");

        public FacilityType Facility { get; private set; }

        private void Start()
        {
            _button.onClick.AddListener(() => FacilityPlacer.SelectFacilityToPlace(Facility));
        }

        private void Update()
        {
            _animator.SetBool(animSelectionProperty, FacilityPlacer.SelectedFacilityType == Facility);
        }

        public void AssignFacility(FacilityType facilityTypeToAssign)
        {
            Facility = facilityTypeToAssign;
            _iconImage.sprite = Facility.icon;
        }

        public ToolTipMessage ToToolTipMessage()
        {
            return Facility.ToToolTipMessage();
        }
    }
}