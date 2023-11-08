using System.Collections.Generic;
using ResourceSystem.Markets;
using UnityEngine;

namespace ResourceSystem.Productions
{
    public interface IProducer : IEconomicActor
    {
        public IProducer producerSelf { get; }
        public List<ProductionLine> productionLines { get; }

        public void FetchResources()
        {
            foreach (var line in productionLines)
            {
                foreach (var demand in line.demands)
                {
                    SetOrder(demand.resource, demand.quantity, OrderType.Demand);
                }
            }
        }

        public void Produce()
        {
            foreach (var line  in productionLines)
            {
                line.SetEfficiency(market);

                foreach (var offer in line.offers)
                {
                    float modifiersBonus = market.modifierContainerSelf.GetMultiplier(offer.resource) +
                                           MarketManager.Instance.modifierContainerSelf.GetMultiplier(offer.resource);
                    float flatProduction = offer.quantity * line.efficiency;
                    SetOrder(offer.resource, flatProduction + flatProduction * modifiersBonus, OrderType.Offer);
                }
            }
        }
    }
}