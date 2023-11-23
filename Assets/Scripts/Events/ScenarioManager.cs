using System.Collections.Generic;
using TimeSystem;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Events
{
    public class ScenarioManager : Singleton<ScenarioManager>
    {
        [SerializeField] private Scenario _scenario;

        private static readonly List<InGameDate> s_dates = new();
        private static readonly List<GameEvent> s_events = new();

        private void Awake()
        {
            if (_scenario)
                InitEventDate();
        }

        private void OnEnable()
        {
            TimeManager.onMonthBegins += FireEventPresent;
        }

        private void OnDisable()
        {
            TimeManager.onMonthBegins -= FireEventPresent;
        }

        private void InitEventDate()
        {
            foreach (var eventDate in _scenario.Events)
            {

                InGameDate randDate;
                do
                {
                    randDate = new InGameDate(
                        Random.Range(eventDate.minDate.totalMonths, eventDate.maxDate.totalMonths));
                } while (s_dates.Contains(randDate));
                
                s_dates.Add(randDate);
                s_events.Add(eventDate.gameEvent);
            }
        }

        private static void FireEventPresent()
        {
            if (TryGetGameEvent(TimeManager.date, out GameEvent gameEvent))
            {
                gameEvent.Fire();
            }
        }

        public static bool TryGetGameEvent(InGameDate date, out GameEvent gameEvent)
        {
            for (int i = 0; i < s_dates.Count; i++)
            {
                if (s_dates[i] == date)
                {
                    gameEvent = s_events[i];
                    return true;
                }
            }

            gameEvent = null;
            return false;
        }
        
        public static List<GameEvent> GetEventsOfYear(int year)
        {
            List<GameEvent> yearEvents = new();

            for (int i = 0; i < s_dates.Count; i++)
            {
                if (s_dates[i].year == year)
                    yearEvents.Add(s_events[i]);
            }

            return yearEvents;
        }
        
        public static List<(int, GameEvent)> GetEventsAndIndicesOfYear(int year)
        {
            List<(int, GameEvent)> yearEvents = new();

            for (int i = 0; i < s_dates.Count; i++)
            {
                if (s_dates[i].year == year)
                    yearEvents.Add((i, s_events[i]));
            }

            return yearEvents;
        }

        public static InGameDate GetDateOfEvent(int eventIndex)
        {
            return eventIndex < s_dates.Count ? s_dates[eventIndex] : default;
        }
    }
}