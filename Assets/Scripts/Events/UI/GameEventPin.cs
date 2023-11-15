using ToolTipSystem;
using ToolTipSystem.Messages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Events.UI
{
    public class GameEventPin : MonoBehaviour, ITooltipMessenger, IPointerEnterHandler,  IPointerExitHandler
    {
        [SerializeField] private TextTooltipMessageUI _textTooltip;
        [SerializeField] private Image _icon;
        private GameEvent _gameEvent;
        public ITooltipMessenger tooltipMessengerSelf => this;
        public TooltipMessageUI tooltipMessage => _textTooltip;

        public GameEvent GameEvent
        {
            get => _gameEvent;
            set
            {
                _gameEvent = value;
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            _icon.sprite = _gameEvent.icon;
        }
        
        public void UpdateTooltipMessage(TooltipMessageUI messageUI)
        {
            if (messageUI is TextTooltipMessageUI textTooltipMessageUI)
            {
                textTooltipMessageUI.SetTexts(_gameEvent.Title, _gameEvent.Summary);
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