namespace ResourceSystem.Markets
{
    public class ResourceValue
    {
        public ResourceType resource;
        public float demand = 0;
        public float offer = 0;
        public float excess = 0;
        public float availability = 0;
        public ResourceAvailability availabilityState;

        public ResourceValue(ResourceType resource)
        {
            this.resource = resource;
        }
    }
}