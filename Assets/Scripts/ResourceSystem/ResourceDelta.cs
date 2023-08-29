namespace ResourceSystem
{
    public struct ResourceDelta
    {
        public ResourceType resource;
        public float monthDelta;
        public float availabilityDelta;
        public float maxDelta;

        public ResourceDelta(ResourceType resource, float monthDelta, float availabilityDelta, float maxDelta)
        {
            this.resource = resource;
            this.monthDelta = monthDelta;
            this.availabilityDelta = availabilityDelta;
            this.maxDelta = maxDelta;
        }
    }
}