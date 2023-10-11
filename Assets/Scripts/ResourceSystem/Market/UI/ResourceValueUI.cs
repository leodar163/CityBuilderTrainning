using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ResourceSystem.Market.UI
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
        
        public void UpdateDisplay()
        {
            _demand.text = _resourceValue.demand.ToString(CultureInfo.InvariantCulture);
            _offer.text = _resourceValue.offer.ToString(CultureInfo.InvariantCulture);
            _ratio.text = _resourceValue.ratio.ToString(CultureInfo.InvariantCulture);
            _resourceIcon.sprite = _resourceValue.resource.icon;
        }
    }
}