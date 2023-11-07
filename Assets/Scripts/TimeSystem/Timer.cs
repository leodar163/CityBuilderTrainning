using System;

namespace TimeSystem
{
    public class Timer
    {

        private readonly int _duration;
        private readonly UpdateMoment _updateMoment;
        private int _remainingDuration;

        public event Action onTimerFinished;
        
        public Timer(int duration, UpdateMoment updateMoment)
        {
            _duration = duration;
            _updateMoment = updateMoment;
        }

        public Timer StartTimer()
        {
            if (_duration <= 0)
            {
                onTimerFinished = null;
                return null;
            }

            _remainingDuration = _duration;
            switch (_updateMoment)
            {
                case UpdateMoment.OnMonthEnds:
                    TimeManager.onMonthEnds += DecrementDuration;
                    break;
                case UpdateMoment.OnNewMonth:
                    TimeManager.onNewMonth += DecrementDuration;
                    break;
                case UpdateMoment.OnMonthBegins:
                    TimeManager.onMonthBegins += DecrementDuration;
                    break;
                case UpdateMoment.OnYearEnds:
                    TimeManager.onYearEnds += DecrementDuration;
                    break;
                case UpdateMoment.OnNewYear:
                    TimeManager.onNewYear += DecrementDuration;
                    break;
                case UpdateMoment.OnYearBegins:
                    TimeManager.onYearBegins += DecrementDuration;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return this;
        }
        
        private void DecrementDuration()
        {
            _remainingDuration--;
            if (_remainingDuration > 0) return;
            onTimerFinished?.Invoke();
            onTimerFinished = null;
        }
    }
}