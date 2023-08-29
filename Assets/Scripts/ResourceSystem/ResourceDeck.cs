using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResourceSystem
{
    [Serializable]
    public class ResourceDeck
    {
        [SerializeField] private List<ResourceSlider> _resourceSliders = new();
        public List<ResourceSlider> resourceSliders => _resourceSliders;

        private List<IResourceModifier> _resourceModifiers = new();

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
                slider.ApplyDelta();
            }
        }
    }
}