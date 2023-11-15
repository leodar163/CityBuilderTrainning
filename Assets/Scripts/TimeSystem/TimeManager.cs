using System;
using UnityEngine;
using UnityEngine.Localization;
using Utils;

namespace TimeSystem
{
    public class TimeManager : Singleton<TimeManager>
    {
        public static int timeSpeed = 1;
        public static bool isPaused;
        public const float secondPerMonth = 5;
        
        [Header("Localization")] 
        [SerializeField] private LocalizedString _localizedMonth;
        [SerializeField] private LocalizedString _localizedYear;
        [SerializeField] private LocalizedString _localizedPrevision;

        public static string monthName => Instance._localizedMonth.GetLocalizedString();
        public static string yearName => Instance._localizedYear.GetLocalizedString();
        public static string previsionName => Instance._localizedPrevision.GetLocalizedString();

        public static event Action onMonthEnds;
        public static event Action onNewMonth;
        public static event Action onMonthBegins;
        public static event Action onYearEnds;
        public static event Action onNewYear;
        public static event Action onYearBegins;
        public static InGameDate date { get; private set; }

        public static float monthTimer { get; private set; }

        private bool newMonth;

        private void Awake()
        {
            date = new InGameDate();
            timeSpeed = 1;
            isPaused = false;
            monthTimer = 0;
        }
        
        private void Update()
        {

            if (isPaused) return;
            
            monthTimer += Time.deltaTime * timeSpeed;
            
            if (newMonth)
            {
                OnMonthBegins();
                newMonth = false;
            }
            
            if (monthTimer >= secondPerMonth)
            {
                monthTimer = 0;

                OnMonthEnds();
                
                OnNewMonth();
            }
        }

        private static void OnMonthEnds()
        {
            onMonthEnds?.Invoke();
            if((date.totalMonths + 1) % 12 == 0) onYearEnds?.Invoke();
        }
        
        private static void OnNewMonth()
        {
            date++;
            onNewMonth?.Invoke();
            if(date.totalMonths % 12 == 0) onNewYear?.Invoke();
            Instance.newMonth = true;
        }

        private static void OnMonthBegins()
        {
            onMonthBegins?.Invoke();
            if((date.totalMonths) % 12 == 0) onYearBegins?.Invoke();   
        }

        public void SetTimeSpeed(int speed)
        {
            timeSpeed = speed;
        }

        public void IncreaseTimeSpeed(int delta)
        {
            if(timeSpeed + delta <= 0) SwitchPause();
            timeSpeed = Mathf.Clamp(timeSpeed + delta, 1, 3);
        }

        public static void SwitchPause()
        {
            isPaused = !isPaused;
        }
    }
}