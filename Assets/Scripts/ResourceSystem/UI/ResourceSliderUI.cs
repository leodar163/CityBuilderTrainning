using System;
using System.Globalization;
using TimeSystem;
using TMPro;
using ToolTipSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ResourceSystem.UI
{
    public class ResourceSliderUI : MonoBehaviour, IToolTipSpeaker
    {
        private ResourceSlider _resourceSlider;
        public ResourceSlider resourceSlider
        {
            get => _resourceSlider;
            set
            {
                _resourceSlider = value;
                UpdateDisplay();
            }
        }

        public ResourceType resource;
        [Space] 
        [SerializeField] private Image _resourceIcon;
        [SerializeField] private TextMeshProUGUI _resourceName;
        [SerializeField] private TextMeshProUGUI _value;
        [SerializeField] private TextMeshProUGUI _maxValue;

        private Action<InGameDate> update;

        public UnityEvent<float> onUpdateValue = new();
        public UnityEvent<float> onUpdateMaxValue = new();
        public UnityEvent<float> onUpdateAvailableValue = new();

        private void Awake()
        {
            update =  _ => UpdateDisplay();
        }

        private void OnEnable()
        {
            TimeManager.onNewMonth += update;
        }

        private void OnDisable()
        {
            TimeManager.onNewMonth -= update;
        }

        private void UpdateDisplay()
        {
            if (_resourceSlider == null)
            {
                if (_resourceName) _resourceName.text = "Null";
                if (_maxValue) _maxValue.text = "1";
                if (_value) _value.text = "0";
            }
            else
            {
                onUpdateMaxValue.Invoke(_resourceSlider.maxQuantity);
                onUpdateValue.Invoke(_resourceSlider.nativeQuantity);
                if(resource.borrowable) onUpdateAvailableValue.Invoke(_resourceSlider.availableQuantity);
                
                if(_resourceName) _resourceName.text = _resourceSlider.resource.resourceName;
                if (_maxValue) _maxValue.text = _resourceSlider.maxQuantity.ToString(CultureInfo.InvariantCulture);
                if (_value) _value.text = _resourceSlider.totalQuantity.ToString(CultureInfo.InvariantCulture);

                if (_resourceIcon) _resourceIcon.sprite = resource.icon;
            }
        }

        [SerializeField] private bool debug;

        private void Update()
        {
            if(debug && Input.GetKeyUp(KeyCode.O))
                print(_resourceSlider.totalQuantity);
        }

        public ToolTipMessage ToToolTipMessage()
        {
            return _resourceSlider.ToToolTipMessage();
        }
    }
}