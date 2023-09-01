using ResourceSystem.UI;
using UnityEngine;
using Utils.UI;

namespace ResourceSystem.Global.UI
{
    public class GlobalResourceDeckUI : PanelUI<GlobalResourceDeckUI>
    {
        [SerializeField] private ResourceSliderUI[] _sliders; 

        protected void Start()
        {
            foreach (var slider in _sliders)
            {
                slider.resourceSlider = GlobalResourceDeck.deck.GetSlider(slider.resource);
            }
        }
    }
}