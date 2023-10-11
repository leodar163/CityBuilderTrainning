namespace ResourceSystem.Market
{
    public class ResourceValue
    {
        public ResourceType resource;
        public float demand = 0;
        public float offer = 0;
        public float ratio = 0;

        public ResourceValue(ResourceType resource)
        {
            this.resource = resource;
        }
    }
}