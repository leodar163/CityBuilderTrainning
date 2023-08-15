using System;
using UnityEngine;
using UnityEngine.UI;

namespace BuildingSystem.Facilities.UI
{
    public class FacilityInfoUI : MonoBehaviour
    {
        private Facility _facility;
        
        public Facility facility
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
        
        
    }
}