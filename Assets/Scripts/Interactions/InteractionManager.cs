using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingSystem.Facilities.UI;
using UnityEngine;
using Utils.UI;

namespace Interactions
{
    public class InteractionManager : Utils.Singleton<InteractionManager>
    {
        private InteractionModeControls _controls;

        [SerializeField] private MonoBehaviour[] _interactorsBase;
        private static List<IInteractor> s_interactors = new();
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

        private void OnValidate()
        {
            List<MonoBehaviour> interactors = new();

            foreach (var mono in _interactorsBase)
            {
                if (mono is IInteractor && !interactors.Contains(mono))
                {
                    interactors.Add(mono);
                }
            }

            _interactorsBase = interactors.ToArray();
        }

        private void Awake()
        {
            _controls = new InteractionModeControls();

            s_interactors.Clear();

            foreach (var mono in _interactorsBase)
            {
                if (mono is IInteractor interactor && !s_interactors.Contains(interactor))
                {
                    s_interactors.Add(interactor);
                }
            }
        }

        private void Start()
        {
            if (TryGetInteractor(defaultMode, out _currentInteractor))
            {
                _currentInteractor.ActivateMode();
            }
            else
            {
                throw new NullReferenceException(
                    $"Miss a default interactor registered in {name} to properly initialize");
            }
        }

        private void Update()
        {
            if (_controls.InteractionMode.Return.WasReleasedThisFrame())
            {
                if (IPanel.focusedPanel != null)
                {
                    IPanel.focusedPanel.ClosePanel();
                }
                else if (_currentInteractor.interactionMode != defaultMode)
                {
                    ReturnToDefaultInteractor();
                }
            }

            if (_controls.InteractionMode.OpenMarketVue.WasReleasedThisFrame())
            {
                if (_currentInteractor.interactionMode != InteractionMode.MarketVue)
                {
                    SwitchInteractionMode(InteractionMode.MarketVue);
                }
                else
                {
                    ReturnToDefaultInteractor();
                }
                
            }

            if (_controls.InteractionMode.OpenFacilityBuildingMode.WasReleasedThisFrame())
            {
                FacilityBuildingPanelUI.Instance.SwitchPanelOpening();
            }
        }

        private void LateUpdate()
        {
            if (_currentInteractor is { interactionMode: InteractionMode.FacilityPlacing, isActive: false })
            {
                ReturnToDefaultInteractor();
            }
        }

        public static bool TryGetInteractor(InteractionMode mode, out IInteractor interactor)
        {
            foreach (var _interactor in s_interactors)
            {
                if (_interactor.interactionMode != mode) continue;
                interactor = _interactor;
                return true;
            }

            interactor = null;
            return false;
        }
        
        public static void SwitchInteractionMode(InteractionMode mode)
        {
            if (TryGetInteractor(mode, out IInteractor interactor))
            {
                SwitchInteractionMode(interactor);
            }
            else
            {
                throw new NullReferenceException(
                    $"Try to switch to interaction mode '{Enum.GetName(typeof(InteractionMode), mode)}'" +
                    $" but there is not interactor registered in {Instance.name}");
            }
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