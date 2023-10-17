using ToolTipSystem;
using UnityEngine;
using UnityEngine.UI;

namespace BuildingSystem.Facilities.UI
{
    public class FacilityBuildingButton : MonoBehaviour, IToolTipSpeaker
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Button _button;
        [SerializeField] private Animator _animator;
        
        private static readonly int animSelectionProperty = Animator.StringToHash("IsSelected");

        private ConstructionSite _constructionSite;

        public FacilityType facility { get; private set; }

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
            _constructionSite = facility as ConstructionSite;

            _iconImage.sprite = _constructionSite ? _constructionSite._facilityToBuild.icon : facility.icon;
        }

        public ToolTipMessage ToToolTipMessage()
        {
            return _constructionSite
                ? _constructionSite._facilityToBuild.ToToolTipMessage()
                : facility.ToToolTipMessage();
        }
    }
}