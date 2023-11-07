
namespace ResourceSystem.Markets
{
    public class ResourceOrder
    {
        public ResourceType resource;
        public float quantity;

        public Market market;
        public IEconomicActor sender;

        public OrderType type;

        public ResourceOrder(ResourceType resource, float quantity, Market market, IEconomicActor sender, OrderType type)
        {
            this.resource = resource;
            this.quantity = quantity;
            this.market = market;
            this.sender = sender;
            this.type = type;
        }
    }
}