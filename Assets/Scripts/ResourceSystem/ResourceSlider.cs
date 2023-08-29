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
        public float maxQuantity;
        [SerializeField] private float _quantity;
        private float _borrowedQuantity = 0;
        private List<IResourceModifier> _modifiers = new ();
        private readonly Dictionary<IResourceBorrower, float> _borrowers = new();

        public float availableQuantity => _quantity - _borrowedQuantity;
        
        public float quantity 
        { 
            get => _quantity;
            set
            {
                _quantity = Mathf.Round(Mathf.Clamp(value, 0, maxQuantity) * 100) / 100;
                if (resource.borrowable && _borrowedQuantity > _quantity)
                    CallBackQuantity(_borrowedQuantity - _quantity);
            }
        }

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

        public float GetNexResourceDelta()
        {
            float delta = 0;

            foreach (var modifier in _modifiers)
            {
                List<ResourceQuantity> resourceDeltas = modifier.GetResourceDelta();
                if(resourceDeltas == null)
                    break;
                foreach (var resourceDelta in resourceDeltas)
                {
                    if (resourceDelta.resource == resource)
                    {
                        delta += resourceDelta.quantity;
                    }
                }
            }

            return Mathf.Round(delta * 100) / 100;
        }
        
        public float GetNexResourceDelta(out string message)
        {
            float delta = 0;

            message = "";

            foreach (var modifier in _modifiers)
            {
                foreach (var resourceDelta in modifier.GetResourceDelta())
                {
                    if (resourceDelta.resource == resource)
                    {
                        delta += resourceDelta.quantity;
                        string deltaColor = ColorUtility.ToHtmlStringRGBA(resourceDelta.quantity == 0 ? Color.white :
                            resourceDelta.quantity > 0 ? Color.green : Color.red);
                        message += $"{modifier.modifierName} : <color=#{deltaColor}>{resourceDelta.quantity}</color>\n";
                    }
                }
            }

            return Mathf.Round(delta * 100) / 100;
        }
        
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

        public ToolTipMessage ToToolTipMessage()
        {
            float delta = GetNexResourceDelta(out string message);
            string deltaColor =
                ColorUtility.ToHtmlStringRGBA(delta == 0 ? Color.white : delta > 0 ? Color.green : Color.red);
            return new ToolTipMessage
            {
                title = resource.resourceName,
                message = $"<color=#{deltaColor}>{(delta > 0 ? "+" : "")}{delta}</color>\n" + message
            };
        }

        public void ApplyDelta()
        {
            quantity += GetNexResourceDelta();
        }

        public float BorrowQuantity(float quantityToBorrow, IResourceBorrower borrower)
        {
            if (!resource.borrowable) return 0;
            
            float quantityBorrowable = availableQuantity < quantityToBorrow ? availableQuantity : quantityToBorrow;

            _borrowers.TryAdd(borrower, 0);

            _borrowers[borrower] += quantityBorrowable;
            _borrowedQuantity += quantityBorrowable;

            return quantityBorrowable;
        }

        public void ReturnQuantity(float QuantityToReturn, IResourceBorrower borrower)
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

        private void CallBackQuantity(float quantityToReturn)
        {
            foreach (var pair in _borrowers)
            {
                var quantityReturnable = pair.Value < quantityToReturn ? pair.Value : quantityToReturn;
                pair.Key.ReleaseResource(quantityReturnable, this);
                quantityToReturn -= quantityReturnable;
            }
        }
    }
}