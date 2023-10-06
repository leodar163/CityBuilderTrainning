using System.Collections.Generic;
using BuildingSystem.Facilities.UI;
using GridSystem;
using UnityEngine;
using Utils.UI;

namespace Interactions
{
    public class InteractionManager : Utils.Singleton<InteractionManager>
    {
        private InteractionModeControls _controls;

        private static List<IInteractor> s_interactors;
        private static IInteractor _currentInteractor;

        [SerializeField] private InteractionMode defaultMode = InteractionMode.GridInteraction;

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
            IInteractor.onCreated += interactor =>
            {
                if (!s_interactors.Contains(interactor))
                {
                    s_interactors.Add(interactor);
                }
            };
            
            _currentInteractor = GridInteractor.Instance;
            _currentInteractor.ActivateMode();
        }

        private void Update()
        {
            if (_controls.InteractionMode.Return.WasReleasedThisFrame())
            {
                if (_currentInteractor.interactionMode != defaultMode)
                {
                    ReturnToDefaultInteractor();
                }
                else if (IPanel.focusedPanel != null)
                {
                    IPanel.focusedPanel.ClosePanel();
                }
            }

            if (_controls.InteractionMode.OpenFacilityBuildingMode.WasReleasedThisFrame())
            {
                FacilityBuildingPanelUI.Instance.SwitchPanelOpening();
            }
        }

        private void LateUpdate()
        {
            if (_currentInteractor.interactionMode ==  InteractionMode.FacilityPlacing && !_currentInteractor.isActive)
            {
                ReturnToDefaultInteractor();
            }
        }

        public static void SwitchInteractionMode(InteractionMode mode)
        {
            SwitchInteractionMode(GetInteractor(mode));
        }

        public static IInteractor GetInteractor(InteractionMode mode)
        {
            foreach (var interactor in s_interactors)
            {
                if (interactor.interactionMode == mode)
                    return interactor;
            }

            return null;
        }
        
        public static void SwitchInteractionMode(IInteractor mode)
        {
            if (mode.isActive || mode == _currentInteractor) return;
            
            _currentInteractor.DeactivateMode();
            _currentInteractor = mode;
            _currentInteractor.ActivateMode();
        }

        public static void ReturnToDefaultInteractor()
        {
            SwitchInteractionMode(Instance.defaultMode);
        }
    }
}