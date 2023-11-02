using BuildingSystem.Facilities.UI;
using Cameras;
using GridSystem;
using GridSystem.Interaction;
using TimeSystem;
using UnityEngine;
using Utils.UI;

namespace InputSystem
{
    public class InputManager : Utils.Singleton<InputManager>
    {
        private GameInputs _controls;

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
            _controls = new GameInputs();
        }

        private void Update()
        {
            if (_controls.InteractionMode.Return.WasReleasedThisFrame())
            {
                if (GridEventSystem.CurrentInteractor.cancelable)
                {
                    GridEventSystem.SwitchInteractor(GridInteractorType.Default);
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
    }
}