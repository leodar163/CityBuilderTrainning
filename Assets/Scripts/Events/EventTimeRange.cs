using System;
using TimeSystem;

namespace Events
{
    [Serializable]
    public struct EventTimeRange
    {
        public GameEvent gameEvent;
        public InGameDate minDate;
        public InGameDate maxDate;
    }
}