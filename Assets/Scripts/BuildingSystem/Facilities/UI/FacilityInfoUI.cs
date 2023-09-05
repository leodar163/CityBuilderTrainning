using System;
using ToolTipSystem;
using UnityEngine;
using UnityEngine.UI;

namespace BuildingSystem.Facilities.UI
{
    public class FacilityInfoUI : MonoBehaviour, IToolTipSpeaker
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

        [SerializeField] private Image _facilityIcon;
        [SerializeField] private Button _deleteButton;

        private void Update()
        {
            if (!_facility)
            {
                Destroy(gameObject);
            }
        }

        private void DisplayFacility()
        {
            _facilityIcon.sprite = _facility.icon;
            
            _deleteButton.onClick.AddListener(() =>
            {
                _facility.cell.terrain.RemoveFacility(_facility);
                Destroy(_facility.gameObject);
            });
        }

        public ToolTipMessage ToToolTipMessage()
        {
            return _facility.ToToolTipMessage();
        }
    }
}