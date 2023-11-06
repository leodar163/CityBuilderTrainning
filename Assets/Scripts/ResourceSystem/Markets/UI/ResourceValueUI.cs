using ResourceSystem.Markets.UI.Tooltip;
using ResourceSystem.UI.Tooltip;
using UnityEngine;

namespace ResourceSystem.Markets.UI
{
    public class ResourceValueUI : MonoBehaviour
    {
        [SerializeField] private ResourceType _resource;
        private ResourceValue _resourceValue;
        private Market _market;
        [Header("UI")] 
        [SerializeField] private ResourceIconMessenger _icon;
        [SerializeField] private OrderListMessenger _demand;
        [SerializeField] private OrderListMessenger _offer;
        [SerializeField] private AvailabilityMessenger _availability;

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

        public ResourceType Resource
        {
            get => _resource;
            set
            {
                _resource = value;
                if (_market != null && _market.TryGetResourceValue(_resource, out _resourceValue))
                    UpdateDisplay();
            }
        }
        
        public void UpdateDisplay()
        {
            if (_demand)
            {
                _demand.Market = _market;
                _demand.Resource = _resource;
            }

            if (_offer)
            {
                _offer.Market = _market;
                _offer.Resource = _resource;
            }
            
            if (_availability)
            {
                _availability.ResourceValue = _resourceValue;
            }

            if (_icon)
            {
                _icon.Resource = _resource;
            }
        }
    }
}