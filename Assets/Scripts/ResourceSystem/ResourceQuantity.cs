using System;

namespace ResourceSystem
{
    [Serializable]
    public struct ResourceQuantity
    {
        public ResourceType resource;
        public float quantity;

        public ResourceQuantity(ResourceType resource, float quantity)
        {
            this.resource = resource;
            this.quantity = quantity;
        }
        
        public static ResourceQuantity operator+(ResourceQuantity a, float b)
        {
            return new ResourceQuantity(a.resource, a.quantity + b);
        }
        
        public static ResourceQuantity operator-(ResourceQuantity a, float b)
        {
            return new ResourceQuantity(a.resource, a.quantity - b);
        }
    }
}