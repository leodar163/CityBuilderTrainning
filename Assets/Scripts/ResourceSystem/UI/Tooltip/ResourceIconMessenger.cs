using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ResourceSystem.UI.Tooltip
{
    public class ResourceIconMessenger : MonoBehaviour, ITooltipMessenger, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ResourceType _resource;
        [SerializeField] private Image _resourceIcon;
        [SerializeField] private TextTooltipMessageUI _textTooltipMessageUI;

        private void Awake()
        {
            UpdateIcon();
        }

        public ResourceType Resource
        {
            get => _resource;
            set
            {
                _resource = value;
                UpdateIcon();
            }
        }

        private void UpdateIcon()
        {
            if (!_resourceIcon) return;
            
            _resourceIcon.sprite = _resource == null ? null : _resource.icon;
        }

        public ITooltipMessenger tooltipMessengerSelf => this;
        public TooltipMessageUI tooltipMessage => _textTooltipMessageUI;
        
        public void UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
            if (_resource == null) return;

            if (messageUI is TextTooltipMessageUI textTooltipMessageUI)
            {
                textTooltipMessageUI.SetTexts(_resource.resourceName, _resource.description);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipMessengerSelf.SubToTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipMessengerSelf.UnsubFromTooltip();
        }
    }
}