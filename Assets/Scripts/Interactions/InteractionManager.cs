using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingSystem.Facilities.UI;
using Cameras;
using GridSystem;
using TimeSystem;
using UnityEngine;
using Utils.UI;

namespace Interactions
{
    public class InteractionManager : Utils.Singleton<InteractionManager>
    {
        private GameInputs _controls;

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
            _controls = new GameInputs();

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

            if (_controls.InteractionMode.OpenFacilityBuildingMode.WasReleasedThisFrame())
            {
                FacilityBuildingPanelUI.Instance.SwitchPanelOpening();
            }
            
            ManageCameraInputs();
            
            ManageMapFilterInputs();
            
            ManageTimeInputs();
        }

        private void ManageCameraInputs()
        {
            CameraController.Instance.MoveCamera(_controls.CameraControles.Move.ReadValue<Vector2>(),
                _controls.CameraControles.SpeepUp.IsPressed()); 
            CameraController.Instance.Zoom(_controls.CameraControles.Zooming.ReadValue<float>());
        }
        
        private void ManageMapFilterInputs()
        {
            if (_controls.MapFilter.TerrainVue.WasReleasedThisFrame())
            {
                MapFilter.ShowMapFilter(TileMapType.Terrain);
            }
            
            if (_controls.MapFilter.MarketVue.WasReleasedThisFrame())
            {
                MapFilter.ShowMapFilter(TileMapType.Market);
            }
        }
        
        private void ManageTimeInputs()
        {
            if (_controls.TimeControles.Pause.WasPressedThisFrame())
            { 
                TimeManager.Instance.Pause();
            }
            
            if (_controls.TimeControles.IncreaseTime.WasPressedThisFrame())
            {
                TimeManager.Instance.IncreaseTimeSpeed((int)_controls.TimeControles.IncreaseTime.ReadValue<float>());
            }

            if (_controls.TimeControles.Time1.WasReleasedThisFrame())
            {
                TimeManager.Instance.SetTimeSpeed(1);
            }

            if (_controls.TimeControles.Time2.WasReleasedThisFrame())
            {
                TimeManager.Instance.SetTimeSpeed(2);
            }
            
            if (_controls.TimeControles.Time3.WasReleasedThisFrame())
            {
                TimeManager.Instance.SetTimeSpeed(3);
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