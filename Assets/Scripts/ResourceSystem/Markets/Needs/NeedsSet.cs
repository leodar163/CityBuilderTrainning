using System;
using System.Collections.Generic;
using TimeSystem;
using UnityEngine;

namespace ResourceSystem.Markets.Needs
{
    [Serializable]
    public class NeedsSet : IEconomicActor
    {
        [SerializeField] private ResourceType _popResource;
        [Space]
        [SerializeField] private float _maxHappiness = 100;
        [SerializeField] private float _minHappiness = -100;

        public float maxHappiness => _maxHappiness;
        public float minHappiness => _minHappiness;
        
        [Space] 
        [SerializeField] private float _positiveProdMult = 0.01f;
        [SerializeField] private float _negativeProdMult = 0.01f;
        [Space]
        public List<Need> needs;

        private Market _market;

        public string EconomicActorName => "people_needs";

        public Market market
        {
            get => _market;
            set
            {
                _market = value;
                AdjustNeeds();
            }
        }

        List<ResourceOrder> IEconomicActor.orders { get; } = new();

        public IEconomicActor economicActorSelf => this;
        private float _happinessDelta;
        public float happinessDelta => _happinessDelta;
        
        private float _happiness;
        public float happiness => _happiness;
        public float prodBonus => _happiness > 0 ? _happiness * _positiveProdMult : _happiness * _negativeProdMult;  

        #region CONSTRUCTORS

        public NeedsSet(NeedsSet template, Market market)
        {
            _popResource = template._popResource;
            needs = new List<Need>(template.needs);
            _market = market;

            _minHappiness = template._minHappiness;
            _maxHappiness = template._maxHappiness;
            _positiveProdMult = template._positiveProdMult;
            _negativeProdMult = template._negativeProdMult;
            
            economicActorSelf.SetOrder(_popResource, 10, OrderType.Offer);

            TimeManager.onMonthEnds += AdjustNeeds;
        }

        public void OnRemovedFromMarket()
        {
            economicActorSelf.RemoveAllOrders();
            _market = null;
            TimeManager.onMonthEnds -= AdjustNeeds;
            needs.Clear();
            needs = null;
            
            economicActorSelf.SetOrder(_popResource, 0, OrderType.Offer);
        }
        #endregion

        public readonly Dictionary<ResourceType, float> _happinessDeltaDetails = new();

        public void AdjustNeeds()
        {
            if (_market is { isEcosystem: true }) return;
            
            float popAmount = _market.GetResourceValueAmount(_popResource, OrderType.Offer);

            if (popAmount == 0) return;
            
            foreach (var need in needs)
            {
                economicActorSelf.SetOrder(need.resource, need.quantity * popAmount, OrderType.Demand);
                
            }

            _happinessDelta = 0;
            
            foreach (var need in needs)
            {
                float resourceDelta = _market.GetResourceValueAmount(need.resource, OrderType.Offer) - need.quantity *
                    popAmount;
                _happinessDelta += resourceDelta;
                if (!_happinessDeltaDetails.TryAdd(need.resource, _happinessDelta))
                {
                    _happinessDeltaDetails[need.resource] = resourceDelta;
                }
            }

            _happiness = Mathf.Clamp(happiness + _happinessDelta, _minHappiness, _maxHappiness);
        }
    }
}