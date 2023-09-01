namespace ResourceSystem
{
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
    }
}