using System;
using GridSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace TimeSystem
{
    public class TimeManager : Utils.Singleton<TimeManager>
    {
        public static int timeSpeed = 1;
        public static bool isPaused;

        public static event Action<InGameDate> onNewMonth;
        public static InGameDate date { get; private set; }
        
        private float monthTimer;

        private TimeControles _controls;


        private void Awake()
        {
            _controls = new TimeControles();
        }

        private void OnEnable()
        {
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }

        private void Update()
        {

            if (_controls.Timer.Pause.WasPressedThisFrame())
            {
                
                Pause();
            }
            
            if (isPaused) return;
            
            monthTimer += Time.deltaTime * timeSpeed;
            
            if (monthTimer >= 6)
            {
                monthTimer = 0;

                date++;
                
                OnNewMonth();
            }

            
            if (_controls.Timer.IncreaseTime.WasPressedThisFrame())
            {
                //print(_controls.Timer.IncreaseTime.ReadValue<float>());
                IncreaseTimeSpeed((int)_controls.Timer.IncreaseTime.ReadValue<float>());
            }

            if (_controls.Timer.Time1.WasReleasedThisFrame())
            {
                SetTimeSpeed(1);
            }

            if (_controls.Timer.Time2.WasReleasedThisFrame())
            {
                SetTimeSpeed(2);
            }
            
            if (_controls.Timer.Time3.WasReleasedThisFrame())
            {
                SetTimeSpeed(3);
            }
        }

        private static void OnNewMonth()
        {
            onNewMonth?.Invoke(date);
        }

        public void SetTimeSpeed(int speed)
        {
            timeSpeed = speed;
        }

        public void IncreaseTimeSpeed(int delta)
        {
            if(timeSpeed + delta <= 0) Pause();
            timeSpeed = Mathf.Clamp(timeSpeed + delta, 1, 3);
        }

        public void Pause()
        {
            isPaused = !isPaused;
        }
    }
}