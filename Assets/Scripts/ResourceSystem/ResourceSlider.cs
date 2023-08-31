using System;
using System.Collections.Generic;
using ToolTipSystem;
using UnityEngine;

namespace ResourceSystem
{
    [Serializable]
    public class ResourceSlider : IToolTipSpeaker
    {
        public ResourceType resource;
        [SerializeField] private float _maxQuantity;
        [SerializeField] private float _quantity;
        private float _borrowedQuantity = 0;
        private List<IResourceModifier> _modifiers = new ();
        private readonly Dictionary<IResourceBorrower, float> _borrowers = new();

        private float _modifierQuantity = 0;
        private float _modifierMaxQuantity = 0;
        
        public float availableQuantity => _quantity - _borrowedQuantity;

        public float maxQuantity
        {
            get => _maxQuantity + _modifierMaxQuantity;
            set => _maxQuantity = Mathf.Round(value * 100) / 100;
        }
        
        public float quantity
        {
            get => _quantity + Mathf.Clamp(_modifierQuantity, 0, maxQuantity);
            set
            {
                _quantity = Mathf.Round(Mathf.Clamp(value, 0, maxQuantity) * 100) / 100;
                if (resource.borrowable && _borrowedQuantity > _quantity)
                    CallBorrowersForRefund(_borrowedQuantity - _quantity);
            }
        }

        #region CONSTRUCTORS

        public ResourceSlider(ResourceSlider template)
        {
            resource = template.resource;
            maxQuantity = template.maxQuantity;
            quantity = template.quantity;
        }
        
        public ResourceSlider(ResourceType resource,  float maxQuantity)
        {
            this.resource = resource;
            this.maxQuantity = maxQuantity;
        }

        public ResourceSlider(string resourceId, float maxQuantity)
        {
            resource = ResourceSet.Default.GetResource(resourceId);
            this.maxQuantity = maxQuantity;
        }

        #endregion

        #region RESOURCE_DELTA_MANAGEMENT

        public float GetNextMonthResourceDelta()
        {
            float delta = 0;

            foreach (var modifier in _modifiers)
            {
                List<ResourceDelta> resourceDeltas = modifier.GetResourceDelta();
                if(resourceDeltas == null)
                    break;
                foreach (var resourceDelta in resourceDeltas)
                {
                    if (resourceDelta.resource == resource)
                    {
                        delta += resourceDelta.monthDelta;
                    }
                }
            }

            return Mathf.Round(delta * 100) / 100;
        }
        
        public float GetNextMonthResourceDelta(out string message)
        {
            float delta = 0;

            message = "";

            foreach (var modifier in _modifiers)
            {
                foreach (var resourceDelta in modifier.GetResourceDelta())
                {
                    if (resourceDelta.resource == resource)
                    {
                        delta += resourceDelta.monthDelta;
                        string deltaColor = ColorUtility.ToHtmlStringRGBA(resourceDelta.monthDelta == 0 ? Color.white :
                            resourceDelta.monthDelta > 0 ? Color.green : Color.red);
                        message += $"{modifier.modifierName} : <color=#{deltaColor}>{resourceDelta.monthDelta}</color>\n";
                    }
                }
            }

            return Mathf.Round(delta * 100) / 100;
        }

        public float GetMaxQuantityFromModifiers()
        {
            float modifiersMaxQuantity = 0;

            foreach (var modifier in _modifiers)
            {
                foreach (var resourceDelta in modifier.GetResourceDelta())
                {
                    if (resourceDelta.resource == resource)
                    {
                        modifiersMaxQuantity += resourceDelta.maxDelta;
                    }
                }
            }

            return modifiersMaxQuantity;
        }

        public float GetQuantityFromModifiers()
        {
            float modifiersQuantity = 0;

            foreach (var modifier in _modifiers)
            {
                foreach (var resourceDelta in modifier.GetResourceDelta())
                {
                    if (resourceDelta.resource == resource)
                    {
                        modifiersQuantity += resourceDelta.quantityDelta;
                    }
                }
            }

            return modifiersQuantity;
        }
        
        public void ApplyMonthDelta()
        {
            _modifierMaxQuantity = GetMaxQuantityFromModifiers();
            quantity += GetNextMonthResourceDelta();
            _modifierQuantity = GetQuantityFromModifiers();
        }

        #endregion

        #region MODIFIER_SUBSCRIPTION

        public void Sub(IResourceModifier modifier)
        {
            if(!_modifiers.Contains(modifier))
                _modifiers.Add(modifier);
        }

        public void Unsub(IResourceModifier modifier)
        {
            if (_modifiers.Contains(modifier))
                _modifiers.Remove(modifier);
        }

        #endregion

        #region TOOLTIP

        public ToolTipMessage ToToolTipMessage()
        {
            float delta = GetNextMonthResourceDelta(out string message);
            string deltaColor =
                ColorUtility.ToHtmlStringRGBA(delta == 0 ? Color.white : delta > 0 ? Color.green : Color.red);
            return new ToolTipMessage
            {
                title = resource.resourceName,
                message = $"<color=#{deltaColor}>{(delta > 0 ? "+" : "")}{delta}</color>\n" + message
            };
        }

        #endregion

        #region LOANING

        public float LoanQuantity(float quantityToBorrow, IResourceBorrower borrower)
        {
            if (!resource.borrowable) return 0;
            
            float quantityBorrowable = availableQuantity < quantityToBorrow ? availableQuantity : quantityToBorrow;

            _borrowers.TryAdd(borrower, 0);

            _borrowers[borrower] += quantityBorrowable;
            _borrowedQuantity += quantityBorrowable;

            return quantityBorrowable;
        }

        public void RefundQuantity(float QuantityToReturn, IResourceBorrower borrower)
        {
            if (!resource.borrowable || !_borrowers.TryGetValue(borrower, out float borrowedQuantity)) return;

            QuantityToReturn = borrowedQuantity < QuantityToReturn ? borrowedQuantity : QuantityToReturn;

            _borrowedQuantity -= QuantityToReturn;
            _borrowers[borrower] -= QuantityToReturn;

            if (_borrowers[borrower] == 0)
            {
                _borrowers.Remove(borrower);
            }
        }

        private void CallBorrowersForRefund(float quantityToReturn)
        {
            foreach (var pair in _borrowers)
            {
                var quantityReturnable = pair.Value < quantityToReturn ? pair.Value : quantityToReturn;
                pair.Key.ReturnResource(quantityReturnable, this);
                quantityToReturn -= quantityReturnable;
            }
        }

        #endregion
    }
}