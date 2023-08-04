using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResourceSystem
{
    [CreateAssetMenu(menuName = "Resource/Resource Deck", fileName = "NewResourceDeck")]
    public class ResourceDeck : ScriptableObject
    {
        public List<ResourceSlider> resourceSliders = new();

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