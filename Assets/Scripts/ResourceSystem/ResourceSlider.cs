using System;
using UnityEngine;

namespace ResourceSystem
{
    [Serializable]
    public class ResourceSlider
    {
        public ResourceType resource;
        public float minQuantity;
        public float maxQuantity;
        [SerializeField] private float _quantity;
            
        public float quantity 
        { 
            get => _quantity;
            set => _quantity = Mathf.Clamp(value, minQuantity, maxQuantity);
        }

        public ResourceSlider(ResourceSlider template)
        {
            resource = template.resource;
            minQuantity = template.minQuantity;
            maxQuantity = template.maxQuantity;
            quantity = template.quantity;
        }
        
        public ResourceSlider(ResourceType resource, float minQuantity, float maxQuantity)
        {
            this.resource = resource;
            this.minQuantity = minQuantity;
            this.maxQuantity = maxQuantity;
        }

        public ResourceSlider(string resourceId, float minQuantity, float maxQuantity)
        {
            resource = ResourceSet.Default.GetResource(resourceId);
            this.minQuantity = minQuantity;
            this.maxQuantity = maxQuantity;
        }
    }
}