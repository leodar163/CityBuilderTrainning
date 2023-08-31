namespace ResourceSystem
{
    public struct ResourceDelta
    {
        public ResourceType resource;
        public float monthDelta;
        public float quantityDelta;
        public float maxDelta;

        public ResourceDelta(ResourceType resource, float monthDelta = 0, float quantityDelta = 0, float maxDelta = 0)
        {
            this.resource = resource;
            this.monthDelta = monthDelta;
            this.maxDelta = maxDelta;
            this.quantityDelta = quantityDelta;
        }
    }
}