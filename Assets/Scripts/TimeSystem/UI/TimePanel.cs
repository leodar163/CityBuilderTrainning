using System.Collections.Generic;
using Events;
using Events.UI;
using TMPro;
using Utils.UI;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace TimeSystem.UI
{
    public class TimePanel : PanelUI<TimePanel>
    {
        [SerializeField] private LocalizedString _localizedPause;
        [Space]
        [SerializeField] private Slider[] _monthSliders;
        [SerializeField] private Button _decreaseButton;
        [SerializeField] private Button _increaseButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private TextMeshProUGUI _pauseSign;
        [SerializeField] private TextMeshProUGUI _timeSpeedMonitor;
        [SerializeField] private TextMeshProUGUI _dateMonitor;

        [Header("Events")] 
        [SerializeField] private GameEventPin _gameEventPinTemplate;
        private readonly List<GameEventPin> _eventPins = new();

        private void Awake()
        {
            _decreaseButton.onClick.AddListener(() => TimeManager.Instance.IncreaseTimeSpeed(-1));
            _increaseButton.onClick.AddListener(() => TimeManager.Instance.IncreaseTimeSpeed(1));
            _pauseButton.onClick.AddListener(TimeManager.Instance.Pause);
            TimeManager.onNewYear += ReinitSliders;
        }

        protected override void Start()
        {
            base.Start();
            ReinitSliders();
        }

        private void Update()
        {
            UpdateSliders();

            _pauseSign.SetText(TimeManager.isPaused ? "|>" : "||");
            _timeSpeedMonitor.text = TimeManager.isPaused ? _localizedPause.GetLocalizedString() : "X" + TimeManager.timeSpeed;
            _dateMonitor.text = TimeManager.date.ToString();
            
        }
        
        private void UpdateSliders()
        {
            int months = TimeManager.date.month;
            if (months > 0)
            {
                for (int i = 0; i < TimeManager.date.month - 1; i++)
                {
                    _monthSliders[i].value = 1;
                }
            }

            _monthSliders[months].value = TimeManager.monthTimer / TimeManager.secondPerMonth;
        }

        private void ReinitSliders()
        {
            UpdateEventPinDisplay();
            foreach (var slider in _monthSliders)
            {
                slider.value = 0;
            }
        }

        #region EVENTS

        private void UpdateEventPinDisplay()
        {
            List<GameEvent> events = ScenarioManager.GetEventsOfYear(TimeManager.date.year);

            foreach (var pin in _eventPins)
            {
                pin.gameObject.SetActive(false);
            }
            
            for (int i = 0; i < events.Count; i++)
            {
                if (_eventPins.Count < i + 1 && Instantiate(_gameEventPinTemplate)
                        .TryGetComponent(out GameEventPin pin))
                {
                    _eventPins.Add(pin);
                }
                else
                {
                    pin = _eventPins[i];
                }

                pin.GameEvent = events[i];
                pin.transform.SetParent(_monthSliders[ScenarioManager.GetDateOfEvent(pin.GameEvent).month].transform);
                ((RectTransform)pin.transform).anchoredPosition = Vector2.zero;
                pin.gameObject.SetActive(true);
            }
        }

        #endregion
    }
}