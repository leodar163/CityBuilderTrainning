using System.Globalization;
using Format;
using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace ResourceSystem.Markets.UI
{
    public class ResourceValueUI : MonoBehaviour
    {
        public Market Market
        {
            get => _market;
            set
            {
                _market = value;
                if (_market == null)
                {
                    _resourceValue = null;
                    return;
                }

                if (_market.TryGetResourceValue(_resource, out _resourceValue))
                    UpdateDisplay();
            }
        }
        
        [SerializeField] private ResourceType _resource;
        private ResourceValue _resourceValue;
        private Market _market;
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _demand;
        [SerializeField] private TextMeshProUGUI _offer;
        [SerializeField] private TextMeshProUGUI _availability;
        [SerializeField] private TextMeshProUGUI _noProduction;
        [SerializeField] private Image _resourceIcon;


        public void UpdateDisplay()
        {
            bool resourceValueExists = _resourceValue != null;
            if (_demand)
            {
                _demand.gameObject.SetActive(resourceValueExists);
                if (resourceValueExists)
                    _demand.SetText(
                        (Mathf.RoundToInt(_resourceValue.demand * 100) / 100f).ToString(CultureInfo.CurrentCulture));
            }

            if (_offer)
            {
                _offer.gameObject.SetActive(resourceValueExists);
                if (resourceValueExists)
                    _offer.SetText(
                        (Mathf.RoundToInt(_resourceValue.offer * 100) / 100f).ToString(CultureInfo.CurrentCulture));
            }  
            if (_availability)
            {
                _availability.gameObject.SetActive(resourceValueExists);
                if (resourceValueExists)
                    _availability.SetText(_resourceValue.availability <= 0
                        ? $"<color=#{FormatManager.negativeColorHTML}>{VariableNameManager.ShortageName}</color>"
                        : Mathf.RoundToInt(_resourceValue.availability * 100) + "%");
            }

            if (_noProduction)
            {
                _noProduction.gameObject.SetActive(!resourceValueExists);
                if (!resourceValueExists)
                    _noProduction.SetText(VariableNameManager.NoResourceValueName);
            }
            
            if (_resourceIcon) _resourceIcon.sprite = _resource.icon;
        }
    }
}