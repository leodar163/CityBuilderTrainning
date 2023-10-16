using System.Collections.Generic;
using GridSystem;

namespace ResourceSystem.Markets.PeoplePleasing
{
    public class PeopleNeeds : IEconomicActor
    {
        public Market market { get; }
        List<ResourceOrder> IEconomicActor.orders { get; } = new();

        public IEconomicActor economicActorSelf => this;
    }
}