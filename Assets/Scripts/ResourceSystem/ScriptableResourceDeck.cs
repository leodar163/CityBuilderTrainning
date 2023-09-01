using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace ResourceSystem
{
    [CreateAssetMenu(menuName = "Resources/Resource Deck", fileName = "NewResourceDeck")]
    public class ScriptableResourceDeck : DefaultableScriptableObject<ScriptableResourceDeck>
    {
        [SerializeField] private List<ResourceSlider> _sliders = new ();

        public ResourceDeck GetResourceDeckCopy()
        {
            ResourceDeck deck = new(ResourceSet.Default);

            foreach (var slider in _sliders)
            {
                ResourceSlider sliderToChange = deck.GetSlider(slider.resource);
                sliderToChange.maxQuantity = slider.maxQuantity;
                sliderToChange.nativeQuantity = slider.nativeQuantity;
            }
            return deck;
        }
    }
}