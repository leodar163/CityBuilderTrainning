using System;
using BuildingSystem.Facilities;
using BuildingSystem.Facilities.UI;
using GridSystem;
using Utils;

namespace Interactions
{
    public class InteractionManager : Singleton<InteractionManager>
    {
        private InteractionModeControls _controls;

        private static IInteractionMode _defaultInteractor;

        private static IInteractionMode _currentInteractor;
        
        private static IInteractionMode _facilityPlacer;

        private void OnEnable()
        {
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }

        private void Awake()
        {
            _controls = new InteractionModeControls();
            _defaultInteractor = GridInteractor.Instance;
            _currentInteractor = _defaultInteractor;
            _currentInteractor.ActivateMode();

            _facilityPlacer = FacilityPlacer.Instance;
            
        }

        private void Update()
        {
            if (_controls.InteractionMode.Return.WasReleasedThisFrame())
            {
                ReturnToDefaultInteractor();
            }

            if (_controls.InteractionMode.OpenFacilityBuildingMode.WasReleasedThisFrame())
            {
                FacilityBuildingPanelUI.Instance.OpenPanel();
            }
        }

        public static void SwitchInteractionMode(InteractionMode mode)
        {
            IInteractionMode interactorToActivate = mode switch
            {
                InteractionMode.Default => _defaultInteractor,
                InteractionMode.FacilityPlacing => _facilityPlacer,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
            
            SwitchInteractionMode(interactorToActivate);
        }

        public static void SwitchInteractionMode(IInteractionMode mode)
        {
            if (mode.isActive || mode == _currentInteractor) return;
            
            _currentInteractor.DeactivateMode();
            _currentInteractor = mode;
            _currentInteractor.ActivateMode();
        }

        public static void ReturnToDefaultInteractor()
        {
            SwitchInteractionMode(_defaultInteractor);
        }
    }
}