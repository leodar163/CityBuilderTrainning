using System;
using System.Globalization;
using TimeSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ResourceSystem.UI
{
    public class ResourceSliderUI : MonoBehaviour
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
        

        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _resourceName;
        [SerializeField] private TextMeshProUGUI _value;
        [SerializeField] private TextMeshProUGUI _minValue;
        [SerializeField] private TextMeshProUGUI _maxValue;

        private Action<InGameDate> update;

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
                _slider.minValue = 0;
                _slider.maxValue = 1;
                _slider.value = 0;
                _resourceName.text = "Null";
                _minValue.text = "0";
                _maxValue.text = "1";
                _value.text = "0";
            }
            else
            {
                _slider.minValue = _resourceSlider.minQuantity;
                _slider.maxValue = _resourceSlider.maxQuantity;
                _slider.value = _resourceSlider.quantity;
                _resourceName.text = _resourceSlider.resource.resourceName;
                _minValue.text = _resourceSlider.minQuantity.ToString(CultureInfo.InvariantCulture);
                _maxValue.text = _resourceSlider.maxQuantity.ToString(CultureInfo.InvariantCulture);
                _value.text = _resourceSlider.quantity.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}