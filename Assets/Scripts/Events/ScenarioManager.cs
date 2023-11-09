using System;
using System.Collections.Generic;
using TimeSystem;
using Utils;
using Random = UnityEngine.Random;

namespace Events
{
    public class ScenarioManager : Singleton<ScenarioManager>
    {
        [NonSerialized] private Scenario _scenario;
        public readonly Dictionary<InGameDate, GameEvent> eventsDico = new();
        
        private void Awake()
        {
            if (_scenario)
                InitEventDate();
        }

        private void OnEnable()
        {
            TimeManager.onMonthBegins += TryFireEvent;
        }

        private void OnDisable()
        {
            TimeManager.onMonthBegins -= TryFireEvent;
        }

        public void InitEventDate()
        {
            foreach (var evnt in _scenario.Events)
            {
                InGameDate date;

                do
                {
                    date = new InGameDate(Random.Range(evnt.minDate.totalMonths, evnt.maxDate.totalMonths));
                } while (!eventsDico.TryAdd(date, evnt.gameEvent));
            }
        }

        private void TryFireEvent()
        {
            if (eventsDico.TryGetValue(TimeManager.date, out GameEvent gameEvent))
            {
                gameEvent.Fire();
            }
        }
        
        public List<GameEvent> GetEventsOfYear(int year)
        {
            List<GameEvent> yearEvents = new();
            
            foreach (var evnt in eventsDico)
            {
                if (evnt.Key.year == year)
                    yearEvents.Add(evnt.Value);
            }

            return yearEvents;
        }
    }
}