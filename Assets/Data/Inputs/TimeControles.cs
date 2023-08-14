//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/Data/Inputs/TimeControles.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @TimeControles: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @TimeControles()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""TimeControles"",
    ""maps"": [
        {
            ""name"": ""Timer"",
            ""id"": ""34f3e68e-4ee2-4b32-8394-650dc594ccab"",
            ""actions"": [
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""f2d6ef5b-dafa-4100-9757-c5798433fc0e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Time1"",
                    ""type"": ""Button"",
                    ""id"": ""00abc552-e8ec-4085-a038-a2c025bd9d9c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Time2"",
                    ""type"": ""Button"",
                    ""id"": ""d4e39ad2-6257-4400-87f6-73ca0919b32d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Time3"",
                    ""type"": ""Button"",
                    ""id"": ""b590b1fa-35c4-4a1b-bc49-a5d007a73bb5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""IncreaseTime"",
                    ""type"": ""Button"",
                    ""id"": ""f2b0445e-e4ad-47c2-bf0e-c84496ed2c34"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c2735d5c-82c0-4dc3-a639-6d2ede72cfcf"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2b5328ff-db5d-462b-b57b-5ce343687a15"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Time1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6cdd2e5c-6c55-49a5-b105-2957adc500fc"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Time2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9b8a7bee-d723-45f6-96a9-4d397a2644c0"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Time3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""NumPad"",
                    ""id"": ""5e7e095d-2615-4129-b7d9-4a0dfad4e503"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""IncreaseTime"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""c441113c-28ea-4c9a-a4ee-ff58ddcc2079"",
                    ""path"": ""<Keyboard>/numpadPlus"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""IncreaseTime"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""131ed97e-8abf-4476-bc83-14a069ee9941"",
                    ""path"": ""<Keyboard>/numpadMinus"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""IncreaseTime"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Test"",
                    ""id"": ""f0b2dec6-6edf-43c5-9641-cff2ac303b4e"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""IncreaseTime"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""73ab77af-cb0e-49da-81d2-b6f4b2ce7699"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""IncreaseTime"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""f11016fe-944c-4092-b881-573205476c34"",
                    ""path"": ""<Keyboard>/semicolon"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""IncreaseTime"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Timer
        m_Timer = asset.FindActionMap("Timer", throwIfNotFound: true);
        m_Timer_Pause = m_Timer.FindAction("Pause", throwIfNotFound: true);
        m_Timer_Time1 = m_Timer.FindAction("Time1", throwIfNotFound: true);
        m_Timer_Time2 = m_Timer.FindAction("Time2", throwIfNotFound: true);
        m_Timer_Time3 = m_Timer.FindAction("Time3", throwIfNotFound: true);
        m_Timer_IncreaseTime = m_Timer.FindAction("IncreaseTime", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Timer
    private readonly InputActionMap m_Timer;
    private List<ITimerActions> m_TimerActionsCallbackInterfaces = new List<ITimerActions>();
    private readonly InputAction m_Timer_Pause;
    private readonly InputAction m_Timer_Time1;
    private readonly InputAction m_Timer_Time2;
    private readonly InputAction m_Timer_Time3;
    private readonly InputAction m_Timer_IncreaseTime;
    public struct TimerActions
    {
        private @TimeControles m_Wrapper;
        public TimerActions(@TimeControles wrapper) { m_Wrapper = wrapper; }
        public InputAction @Pause => m_Wrapper.m_Timer_Pause;
        public InputAction @Time1 => m_Wrapper.m_Timer_Time1;
        public InputAction @Time2 => m_Wrapper.m_Timer_Time2;
        public InputAction @Time3 => m_Wrapper.m_Timer_Time3;
        public InputAction @IncreaseTime => m_Wrapper.m_Timer_IncreaseTime;
        public InputActionMap Get() { return m_Wrapper.m_Timer; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TimerActions set) { return set.Get(); }
        public void AddCallbacks(ITimerActions instance)
        {
            if (instance == null || m_Wrapper.m_TimerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_TimerActionsCallbackInterfaces.Add(instance);
            @Pause.started += instance.OnPause;
            @Pause.performed += instance.OnPause;
            @Pause.canceled += instance.OnPause;
            @Time1.started += instance.OnTime1;
            @Time1.performed += instance.OnTime1;
            @Time1.canceled += instance.OnTime1;
            @Time2.started += instance.OnTime2;
            @Time2.performed += instance.OnTime2;
            @Time2.canceled += instance.OnTime2;
            @Time3.started += instance.OnTime3;
            @Time3.performed += instance.OnTime3;
            @Time3.canceled += instance.OnTime3;
            @IncreaseTime.started += instance.OnIncreaseTime;
            @IncreaseTime.performed += instance.OnIncreaseTime;
            @IncreaseTime.canceled += instance.OnIncreaseTime;
        }

        private void UnregisterCallbacks(ITimerActions instance)
        {
            @Pause.started -= instance.OnPause;
            @Pause.performed -= instance.OnPause;
            @Pause.canceled -= instance.OnPause;
            @Time1.started -= instance.OnTime1;
            @Time1.performed -= instance.OnTime1;
            @Time1.canceled -= instance.OnTime1;
            @Time2.started -= instance.OnTime2;
            @Time2.performed -= instance.OnTime2;
            @Time2.canceled -= instance.OnTime2;
            @Time3.started -= instance.OnTime3;
            @Time3.performed -= instance.OnTime3;
            @Time3.canceled -= instance.OnTime3;
            @IncreaseTime.started -= instance.OnIncreaseTime;
            @IncreaseTime.performed -= instance.OnIncreaseTime;
            @IncreaseTime.canceled -= instance.OnIncreaseTime;
        }

        public void RemoveCallbacks(ITimerActions instance)
        {
            if (m_Wrapper.m_TimerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ITimerActions instance)
        {
            foreach (var item in m_Wrapper.m_TimerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_TimerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public TimerActions @Timer => new TimerActions(this);
    public interface ITimerActions
    {
        void OnPause(InputAction.CallbackContext context);
        void OnTime1(InputAction.CallbackContext context);
        void OnTime2(InputAction.CallbackContext context);
        void OnTime3(InputAction.CallbackContext context);
        void OnIncreaseTime(InputAction.CallbackContext context);
    }
}
