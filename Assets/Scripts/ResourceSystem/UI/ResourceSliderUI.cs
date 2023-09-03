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
                if (_resourceSlider != null)
                    _resourceSlider.onValuesUpdate -= UpdateDisplay;
                
                _resourceSlider = value;
                _resourceSlider.onValuesUpdate += UpdateDisplay;
                UpdateDisplay();
            }
        }

        public ResourceType resource;
        [Space] 
        [SerializeField] private Image _resourceIcon;
        [SerializeField] private TextMeshProUGUI _resourceName;
        [SerializeField] private TextMeshProUGUI _value;

        [Space] 
        [SerializeField] private Image _quantityEvolutionFB;
        [SerializeField] private Sprite _increaseFeedback;
        [SerializeField] private Sprite _decreaseFeedback;
        
        [Header("Events")]
        public UnityEvent<float> onUpdateValue = new();
        public UnityEvent<float> onUpdateMaxValue = new();
        public UnityEvent<float> onUpdateAvailableValue = new();
        [Space] 
        public bool displayMaxQuantity = true;

        public bool displayAvailableQuantity = true;

        private void UpdateDisplay()
        {
            if (_resourceSlider == null)
            {
                if (_resourceName) _resourceName.SetText("Null");
                if (_value) _value.SetText("0");
            }
            else
            {
                onUpdateMaxValue.Invoke(_resourceSlider.maxQuantity);
                onUpdateValue.Invoke(_resourceSlider.nativeQuantity);
                if(resource.borrowable) onUpdateAvailableValue.Invoke(_resourceSlider.availableQuantity);

                if(_resourceName) _resourceName.SetText(_resourceSlider.resource.resourceName);
                if (_value) _value.SetText(FormatValue());

                if (_resourceIcon) _resourceIcon.sprite = resource.icon;
                
                ManageQuantityEvolutionFeedback();
            }
        }

        private string FormatValue()
        {
            string value = "";
            
            if (displayAvailableQuantity && _resourceSlider.resource.borrowable)
                value += _resourceSlider.availableQuantity + "/";
            
            value += _resourceSlider.totalQuantity;
            
            if (displayMaxQuantity && resourceSlider.maxQuantity != float.PositiveInfinity)
                value += "/" + _resourceSlider.maxQuantity;

            return value;
        }

        private void ManageQuantityEvolutionFeedback()
        {
            if(_quantityEvolutionFB == null) return;
                
            float evolution = _resourceSlider.GetNextMonthResourceDelta();
            
            switch (evolution)
            {
                case 0:
                    _quantityEvolutionFB.gameObject.SetActive(false);
                    break;
                case > 0:
                    if(_increaseFeedback)
                    {
                        _quantityEvolutionFB.sprite = _increaseFeedback;
                        _quantityEvolutionFB.gameObject.SetActive(true);
                    }
                    else
                    {
                        _quantityEvolutionFB.gameObject.SetActive(false);
                    }
                    break;
                case < 0:
                    if(_decreaseFeedback)
                    {
                        _quantityEvolutionFB.sprite = _decreaseFeedback;
                        _quantityEvolutionFB.gameObject.SetActive(true);
                    }
                    else
                    {
                        _quantityEvolutionFB.gameObject.SetActive(false);
                    }
                    break;
            }
        }
        
        public ToolTipMessage ToToolTipMessage()
        {
            return _resourceSlider.ToToolTipMessage();
        }
    }
}