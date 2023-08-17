﻿using System;
using UnityEngine;
using Utils;

namespace TimeSystem
{
    public class TimeManager : Singleton<TimeManager>
    {
        public static int timeSpeed = 1;
        public static bool isPaused;
        public const float secondPerMonth = 5;

        public static event Action<InGameDate> onNewMonth;
        public static event Action<InGameDate> onNewYear;
        public static InGameDate date { get; private set; }

        public static float monthTimer { get; private set; }

        private TimeControles _controls;


        private void Awake()
        {
            _controls = new TimeControles();
            date = new InGameDate();
            timeSpeed = 1;
            isPaused = false;
            monthTimer = 0;
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
            
            if (monthTimer >= secondPerMonth)
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
            if(date.totalMonths % 12 == 0) onNewYear?.Invoke(date);
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