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
        public float minQuantity;
        public float maxQuantity;
        [SerializeField] private float _quantity;

        private List<IResourceModifier> _modifiers = new ();
        

        public float quantity 
        { 
            get => _quantity;
            set => _quantity = Mathf.Round(Mathf.Clamp(value, minQuantity, maxQuantity) * 100) / 100;
        }

        public ResourceSlider(ResourceSlider template)
        {
            resource = template.resource;
            minQuantity = template.minQuantity;
            maxQuantity = template.maxQuantity;
            quantity = template.quantity;
        }
        
        public ResourceSlider(ResourceType resource, float minQuantity, float maxQuantity)
        {
            this.resource = resource;
            this.minQuantity = minQuantity;
            this.maxQuantity = maxQuantity;
        }

        public ResourceSlider(string resourceId, float minQuantity, float maxQuantity)
        {
            resource = ResourceSet.Default.GetResource(resourceId);
            this.minQuantity = minQuantity;
            this.maxQuantity = maxQuantity;
        }

        public float GetNexResourceDelta()
        {
            float delta = 0;

            foreach (var modifier in _modifiers)
            {
                foreach (var resourceDelta in modifier.GetResourceDelta())
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
                title = $"<color=#{deltaColor}>{delta}</color>",
                message = message
            };
        }

        public void ApplyDelta()
        {
            quantity += GetNexResourceDelta();
        }
    }
}