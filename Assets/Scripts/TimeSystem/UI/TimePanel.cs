using TMPro;
using Utils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TimeSystem.UI
{
    public class TimePanel : PanelUI<TimePanel>
    {
        [SerializeField] private Slider[] _monthSliders;
        [SerializeField] private Button _decreaseButton;
        [SerializeField] private Button _increaseButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private TextMeshProUGUI _timeSpeedMonitor;
        [SerializeField] private TextMeshProUGUI _dateMonitor;

        protected override void Awake()
        {
            base.Awake();
            _decreaseButton.onClick.AddListener(() => TimeManager.Instance.IncreaseTimeSpeed(-1));
            _increaseButton.onClick.AddListener(() => TimeManager.Instance.IncreaseTimeSpeed(1));
            _pauseButton.onClick.AddListener(TimeManager.Instance.Pause);
            TimeManager.onNewYear += _ => ReinitSliders();
        }

        private void Update()
        {
            UpdateSliders();

            _timeSpeedMonitor.text = "X " + TimeManager.timeSpeed;
            _dateMonitor.text = TimeManager.date.ToString();
        }
        
        private void UpdateSliders()
        {
            int months = TimeManager.date.months;
            if (months > 0)
            {
                for (int i = 0; i < TimeManager.date.months - 1; i++)
                {
                    _monthSliders[i].value = 1;
                }
            }

            _monthSliders[months].value = TimeManager.monthTimer / TimeManager.secondPerMonth;
        }

        private void ReinitSliders()
        {
            foreach (var slider in _monthSliders)
            {
                slider.value = 0;
            }
        }
    }
}