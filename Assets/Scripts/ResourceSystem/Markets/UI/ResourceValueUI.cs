using System.Globalization;
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
            _demand.text = (Mathf.RoundToInt(_resourceValue.demand * 100) / 100f).ToString(CultureInfo.CurrentCulture);
            _offer.text = (Mathf.RoundToInt(_resourceValue.offer * 100) / 100f).ToString(CultureInfo.CurrentCulture);
            _ratio.text = _resourceValue.availability <= 0
                ? $"<color=#{FormatManager.negativeColorHTML}>{_localizedShortage.GetLocalizedString()}</color>"
                //: ((int)(_resourceValue.availability * 100) / 100).ToString(CultureInfo.InvariantCulture);
                : Mathf.RoundToInt(_resourceValue.availability * 100) + "%";
            _resourceIcon.sprite = _resourceValue.resource.icon;
        }
    }
}