﻿using System.Globalization;
using Format;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace ResourceSystem.Markets.UI
{
    public class ResourceValueUI : MonoBehaviour
    {
        private ResourceValue _resourceValue;

        public ResourceValue ResourceValue
        {
            get => _resourceValue;
            set
            {
                _resourceValue = value;
                UpdateDisplay();
            }
        }

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _demand;
        [SerializeField] private TextMeshProUGUI _offer;
        [SerializeField] private TextMeshProUGUI _ratio;
        [SerializeField] private Image _resourceIcon;
        [Header("Localization")] 
        [SerializeField] private LocalizedString _localizedShortage;
        
        public void UpdateDisplay()
        {
            _demand.text = _resourceValue.demand.ToString(CultureInfo.InvariantCulture);
            _offer.text = _resourceValue.offer.ToString(CultureInfo.InvariantCulture);
            _ratio.text = _resourceValue.availability < 0
                ? $"<color=#{FormatManager.negativeColor}>{_localizedShortage.GetLocalizedString()}</color>"
                : _resourceValue.availability.ToString(CultureInfo.InvariantCulture);
            _resourceIcon.sprite = _resourceValue.resource.icon;
        }
    }
}