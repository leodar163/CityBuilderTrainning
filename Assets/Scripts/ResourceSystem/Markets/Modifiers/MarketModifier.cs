using System;
using System.Collections.Generic;
using Effects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ResourceSystem.Markets.Modifiers
{
    [CreateAssetMenu(menuName = "Effects/MarketModifier", fileName = "NewMarketModifier")]
    public class MarketModifier : Effect
    {
        [SerializeField] private MarketModifierScope _scope;
        [SerializeField] private MarketType _marketTypeFilter;
        
        public MarketModifierScope Scope => _scope;
        public MarketType MarketTypeFilter => _marketTypeFilter;

        private IMarketModifierContainer _container;

        [SerializeField] private List<OrderSummary> _orders;
        [SerializeField] private List<ResourceQuantity> _multipliers;

        public List<OrderSummary> Orders => _orders;
        public List<ResourceQuantity> Multipliers => _multipliers;

        public override void Apply()
        {
            base.Apply();
            PickContainer();
            _container?.AddModifier(this);
        }

        public override void Unapply()
        {
            base.Unapply();
            _container?.RemoveModifier(this);
        }

        private void PickContainer()
        {
            _container = _scope switch
            {
                MarketModifierScope.Global => MarketManager.Instance,
                MarketModifierScope.Main => MarketManager.mainMarket,
                MarketModifierScope.Random => _marketTypeFilter switch
                {
                    MarketType.Artificial => MarketManager.artificialMarkets[Random.Range(0, MarketManager.artificialMarkets.Count)],
                    MarketType.Ecosystem => MarketManager.ecosystemMarkets[Random.Range(0, MarketManager.ecosystemMarkets.Count)],
                    _ => MarketManager.markets[Random.Range(0, MarketManager.markets.Count)]
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void OnRemoved()
        {
            _container = null;
        }
    }
}