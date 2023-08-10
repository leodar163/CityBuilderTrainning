using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResourceSystem
{
    [CreateAssetMenu(menuName = "Resources/Resource Deck", fileName = "NewResourceDeck")]
    public class ResourceDeck : ScriptableObject
    {
        [SerializeField] private List<ResourceSlider> _resourceSliders = new();
        public List<ResourceSlider> resourceSliders => _resourceSliders;

        [Serializable]
        public class ResourceSlider
        {
            public ResourceType resource;
            public float minQuantity;
            public float maxQuantity;
            private float _quantity;
            
            public float quantity 
            { 
                get => _quantity;
                set => _quantity = Mathf.Clamp(value, minQuantity, maxQuantity);
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
}