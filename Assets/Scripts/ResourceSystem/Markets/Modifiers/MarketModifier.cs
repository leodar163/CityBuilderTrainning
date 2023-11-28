using System;
using System.Collections.Generic;
using Effects;
using Format;
using Localization;
using UnityEngine;
using UnityEngine.Localization;
using Random = UnityEngine.Random;

namespace ResourceSystem.Markets.Modifiers
{
    [Serializable]
    public class MarketModifier : Effect
    {
        [Space]
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

        #region CONSTRUCTORS

        public MarketModifier(Sprite icon, LocalizedString name, int duration,
            IEnumerable<OrderSummary> orders, IEnumerable<ResourceQuantity> multipliers, MarketModifierScope scope,
            MarketType marketType)
            : base(icon, name, duration)
        {
            _orders = new List<OrderSummary>(orders);
            _multipliers = new List<ResourceQuantity>(multipliers);
            _scope = scope;
            _marketTypeFilter = marketType;
        }

        public MarketModifier(MarketModifier template) : this(template._icon, template._name,
            template._duration, template._orders, template._multipliers, template._scope, template._marketTypeFilter)
        {
        }

        #endregion

        public string GetFormatMessage(bool includeTarget)
        {
            string format = "";
            
            for (int i = 0; i < _orders.Count; i++)
            {
                OrderSummary order = _orders[i];
                if (order.quantity <= 0) continue;
                format += $"+{order.quantity} " +
                          $"{(order.orderType == OrderType.Demand ? $"<color=#{FormatManager.negativeColorHTML}>{VariableNameManager.DemandName}" : $"<color=#{FormatManager.positiveColorHTML}>{VariableNameManager.OfferName}")} " +
                          $"</color>{order.resource.ResourceName}";
                if (includeTarget)
                    format += $" {FormatManager.FormatMarketScope(_marketTypeFilter, _scope)}";
                if (i < _orders.Count - 1)
                    format += '\n';

            }

            if (format.Length > 0 && _multipliers.Count > 0)
                format += '\n' + FormatManager.separator + '\n';

            for (int i = 0; i < _multipliers.Count; i++)
            {
                ResourceQuantity mult = _multipliers[i];
                if (mult.quantity == 0) continue;
                string posNeg = mult.quantity > 0
                    ? $"<color=#{FormatManager.positiveColorHTML}>+"
                    : $"<color=#{FormatManager.negativeColor}>";
                format +=
                    $"{posNeg}{Mathf.RoundToInt(mult.quantity * 1000) / 10f}%</color> " +
                    $"{VariableNameManager.ProductionName} ({mult.resource.ResourceName})";
                if (includeTarget)
                    format += $" {FormatManager.FormatMarketScope(_marketTypeFilter, _scope)}";
                if (i < _multipliers.Count - 1)
                    format += '\n';
            }

            return format;
        }

        public override string GetFormatMessage()
        {
            return GetFormatMessage(true);
        }

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
            if (_container == null)
            {
                Debug.LogError($"Trying to send market modifier {EffectName} without target");
                return;
            }

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