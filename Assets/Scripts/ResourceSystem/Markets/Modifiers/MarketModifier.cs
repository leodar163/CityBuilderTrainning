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

        private IMarketModifierContainer _container;

        [SerializeField] private List<OrderSummary> _orders;
        [SerializeField] private List<ResourceQuantity> _multipliers;
        
        private readonly List<OrdersSender> _modSenders = new();
        public MarketModifierScope Scope => _scope;
        public MarketType MarketTypeFilter => _marketTypeFilter;
        public List<OrderSummary> Orders => _orders;
        public List<ResourceQuantity> Multipliers => _multipliers;

        public void Apply(IMarketModifierContainer container)
        {
            _container = container;
            Apply();
        }

        public override void Apply()
        {
            if (_container == null )
                PickContainer();
            _container?.AddModifier(this);
            SendOrders();
            base.Apply();
        }

        public override void Unapply()
        {
            _container?.RemoveModifier(this);
            base.Unapply();
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

        private void SendOrders()
        {
            if (_container == null) return;

            foreach (var market in _container.Markets)
            {
                if (_marketTypeFilter != MarketType.Both && market.type != _marketTypeFilter) continue;
                
                _modSenders.Add(new OrdersSender(_name, market).SendOrders(_orders));
            }
        }

        private void RecallSenders()
        {
            foreach (var sender in _modSenders.ToArray())
            {
                _modSenders.Remove(sender.SendBackOrders());
            }
        }
        
        public void ResetOrderSending()
        {
            RecallSenders();
            SendOrders();
        }
        
        public void OnRemoved()
        {
            RecallSenders();
            _container = null;
        }
    }
}