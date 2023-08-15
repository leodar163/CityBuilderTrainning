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

        public ResourceDeck(ResourceDeck template)
        {
            foreach (var slider in template._resourceSliders)
            {
                _resourceSliders.Add(new ResourceSlider(slider));
            }
        }
    }
}