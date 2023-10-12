namespace ResourceSystem
{
    public struct ResourceQuantity
    {
        public ResourceType resource;
        public float quantity;

        public ResourceQuantity(ResourceType resource, float quantity)
        {
            this.resource = resource;
            this.quantity = quantity;
        }
    }
}