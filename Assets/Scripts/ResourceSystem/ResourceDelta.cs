using System;

namespace ResourceSystem
{
    [Serializable]
    public struct ResourceDelta
    {
        public ResourceType resource;
        public float monthDelta;
        public float quantityDelta;
        public float maxQuantityDelta;

        public ResourceDelta(ResourceType resource, float monthDelta = 0, float quantityDelta = 0, float maxQuantityDelta = 0)
        {
            this.resource = resource;
            this.monthDelta = monthDelta;
            this.maxQuantityDelta = maxQuantityDelta;
            this.quantityDelta = quantityDelta;
        }
        
        public static ResourceDelta operator*(ResourceDelta delta, float scale)
        {
            return new ResourceDelta
            {
                resource = delta.resource,
                monthDelta = delta.monthDelta * scale,
                quantityDelta = delta.quantityDelta * scale,
                maxQuantityDelta = delta.maxQuantityDelta * scale
            };
        }
        
        public static ResourceDelta operator/(ResourceDelta delta, float scale)
        {
            return new ResourceDelta
            {
                resource = delta.resource,
                monthDelta = delta.monthDelta / scale,
                quantityDelta = delta.quantityDelta / scale,
                maxQuantityDelta = delta.maxQuantityDelta / scale
            };
        }

        public static ResourceDelta operator +(ResourceDelta delta, float added)
        {
            return new ResourceDelta
            {
                resource = delta.resource,
                monthDelta = delta.monthDelta + added,
                quantityDelta = delta.quantityDelta + added,
                maxQuantityDelta = delta.maxQuantityDelta + added
            };
        }        
        
        public static ResourceDelta operator -(ResourceDelta delta, float added)
        {
            return new ResourceDelta
            {
                resource = delta.resource,
                monthDelta = delta.monthDelta - added,
                quantityDelta = delta.quantityDelta - added,
                maxQuantityDelta = delta.maxQuantityDelta - added
            };
        }        
        public static ResourceDelta operator +(ResourceDelta deltaA, ResourceDelta deltaB)
        {
            if (deltaA.resource != deltaB.resource) return deltaA;
            
            return new ResourceDelta
            {
                resource = deltaA.resource,
                monthDelta = deltaA.monthDelta + deltaB.monthDelta,
                quantityDelta = deltaA.quantityDelta + deltaB.quantityDelta,
                maxQuantityDelta = deltaA.maxQuantityDelta + deltaB.maxQuantityDelta
            };
        }
        
        public static ResourceDelta operator -(ResourceDelta deltaA, ResourceDelta deltaB)
        {
            if (deltaA.resource != deltaB.resource) return deltaA;
            
            return new ResourceDelta
            {
                resource = deltaA.resource,
                monthDelta = deltaA.monthDelta - deltaB.monthDelta,
                quantityDelta = deltaA.quantityDelta - deltaB.quantityDelta,
                maxQuantityDelta = deltaA.maxQuantityDelta - deltaB.maxQuantityDelta
            };
        }
    }
}