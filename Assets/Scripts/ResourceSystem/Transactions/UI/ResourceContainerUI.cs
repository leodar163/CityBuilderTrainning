using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ResourceSystem.Transactions.UI
{
    public class ResourceContainerUI : MonoBehaviour
    {
        public ResourceType resource;

        private ResourceContainer _container;

        public ResourceContainer Container
        {
            get => _container;
            set
            {
                _container = value;
                UpdateDisplay();
            }
        }
        
        [Space] 
        [SerializeField] private Image _resourceIcon;
        [SerializeField] private TextMeshProUGUI _resourceName;
        [SerializeField] private TextMeshProUGUI _value;
        [Space] 
        public bool displayAvailableQuantity = true;
        public bool displayMaxQuantity = true;
        
        [Space] 
        [SerializeField] private Image _quantityEvolutionFB;
        [SerializeField] private Sprite _increaseFeedback;
        [SerializeField] private Sprite _decreaseFeedback;
        
        [Header("Events")]
        public UnityEvent<float> onUpdateValue = new();
        public UnityEvent<float> onUpdateMaxValue = new();
        public UnityEvent<float> onUpdateAvailableValue = new();
        

        private void LateUpdate()
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (resource == null || _container == null) return;
            
            if (_resourceIcon != null) _resourceIcon.sprite = resource.icon;
            if (_resourceName != null) _resourceName.text = resource.resourceName;
            if (_value != null) _value.text = FormatValue();

            if (_quantityEvolutionFB != null)
            {
                switch (_container.deltaQuantity)
                {
                    case > 0 when _increaseFeedback != null:
                        _quantityEvolutionFB.sprite = _increaseFeedback;
                        _quantityEvolutionFB.gameObject.SetActive(true);
                        break;
                    case < 0 when _decreaseFeedback != null:
                        _quantityEvolutionFB.sprite = _decreaseFeedback;
                        _quantityEvolutionFB.gameObject.SetActive(true);
                        break;
                    default:
                        _quantityEvolutionFB.gameObject.SetActive(false);
                        break;
                }
            }
            
            onUpdateMaxValue.Invoke(_container.nativeMaxQuantity);
            onUpdateValue.Invoke(_container.totalQuantity);
            onUpdateAvailableValue.Invoke(_container.availableQuantity);
        }

        private string FormatValue()
        {
            string value = "";

            if (displayAvailableQuantity) value += $"{_container.availableQuantity}/";
            value += $"{_container.totalQuantity}";
            if (displayMaxQuantity) value += $"/{_container.totalMaxQuantity}";

            return value;
        }
    }
}