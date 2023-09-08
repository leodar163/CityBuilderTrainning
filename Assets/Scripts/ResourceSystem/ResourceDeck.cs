using System;
using System.Collections.Generic;
using ResourceSystem.Market;
using UnityEngine;

namespace ResourceSystem
{
    [Serializable]
    public class ResourceDeck : IResourceBorrower
    {
        [SerializeField] private List<ResourceSlider> _resourceSliders = new();

        public Dictionary<ResourceSlider, float> loaners { get; } = new();
        public string borrowerName => "deck";
        
        public List<ResourceSlider> resourceSliders => _resourceSliders;
        public ResourceMarket market { get; private set; }
        private List<IResourceModifier> _resourceModifiers = new();
        private List<IResourceModifier> _permanentResourceModifiers = new();

        public ResourceDeck(ResourceDeck template)
        {
            foreach (var slider in template._resourceSliders)
            {
                _resourceSliders.Add(new ResourceSlider(slider));
            }
        }

        public ResourceDeck(ResourceSet set)
        {
            foreach (var resource in set.resources)
            {
                _resourceSliders.Add(new ResourceSlider(resource, 0));       
            }
        }

        public void Sub(IResourceModifier modifier)
        {
            if (!_resourceModifiers.Contains(modifier))
                _resourceModifiers.Add(modifier);

            foreach (var slider in _resourceSliders)
            {
                slider.Sub(modifier);
            }
        }

        public void Unsub(IResourceModifier modifier)
        {
            if (_resourceModifiers.Contains(modifier))
                _resourceModifiers.Remove(modifier);

            foreach (var slider in _resourceSliders)
            {
                slider.Unsub(modifier);
            }
        }

        public ResourceSlider GetSlider(ResourceType resourceType)
        {
            foreach (var slider in _resourceSliders)
            {
                if (slider.resource == resourceType)
                    return slider;
            }

            return null;
        }

        public void ApplyDeltaToSliders()
        {
            foreach (var slider in _resourceSliders)
            {
                slider.ApplyMonthDelta();
            }
        }
    }
}