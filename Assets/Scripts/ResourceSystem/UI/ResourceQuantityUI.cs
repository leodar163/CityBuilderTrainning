using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ResourceSystem.UI
{
    public class ResourceQuantityUI : MonoBehaviour
    {
        [SerializeField] private Image _resourceIcon;
        [SerializeField] private TextMeshProUGUI _quantity;
        
        private ResourceType _resource;

        public ResourceType resource => _resource;

        public void DisplayResourceQuantity(ResourceType resourceType, float quantity, Color color)
        {
            _resource = resourceType;
            _resourceIcon.sprite = resourceType == null ? resourceType.icon : null;
            _quantity.color = color;
            _quantity.SetText(quantity.ToString(CultureInfo.CurrentCulture));
        }
    }
}