using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ResourceSystem.Markets.Modifiers.UI
{
    public class MarketModifierUI : MonoBehaviour
    {
        private MarketModifier _marketModifier;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _name;

        public MarketModifier Modifier
        {
            get => _marketModifier;
            set
            {
                _marketModifier = value;
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            if (_icon) _icon.sprite = _marketModifier.Icon;
            if (_name) _name.SetText(_marketModifier.EffectName);
        }
    }
}