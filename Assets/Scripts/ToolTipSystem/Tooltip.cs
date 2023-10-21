using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

namespace ToolTipSystem
{
    public class Tooltip : Singleton<Tooltip>
    {
        [SerializeField] private RectTransform _child; 
        [SerializeField] private Vector2 _offset = new (10,0);
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Messages")] 
        [SerializeField] private RectTransform _messageLayout;
        [SerializeField] private List<TooltipMessageUI> _toolTipMessageUIs;
        private static TooltipMessageUI s_currentMessageUI;

        private static ITooltipMessenger s_currentMessenger;

        [Header("Timers")]
        public float timeToShowToolTip = 0.1f;
        public float timeToShowMessage = 0.5f;

        private float _timeBeingOpened;

        private static bool _tooltipIsShown;
        private static bool _messageIsShown;

        private static bool isOpen;

        private void LateUpdate()
        {
            if (s_currentMessenger != null)
                s_currentMessenger.UpdateTooltipMessage(s_currentMessageUI);
            
            if (isOpen)
            {
                _timeBeingOpened += Time.deltaTime;
                
                if (!_tooltipIsShown && Instance._timeBeingOpened >= Instance.timeToShowToolTip)
                    Instance.ShowToolTip();
                if (!_messageIsShown && Instance._timeBeingOpened >= 
                    Instance.timeToShowToolTip + Instance.timeToShowMessage)
                    Instance.ShowMessage();
                
                PlaceTooltip(Input.mousePosition);
            }
        }
        
        private static void CloseToolTip()
        {
            isOpen = false;
            _tooltipIsShown = false;
            _messageIsShown = false;

            Instance._timeBeingOpened = 0;
            
            Instance._canvasGroup.alpha = 0;
            s_currentMessageUI?.Hide();
        }

        private static void OpenToolTip()
        {
            isOpen = true;
        }

        private void PlaceTooltip(Vector2 screenPosition)
        {
            Vector3 newPos = screenPosition;
            newPos.z = _child.position.z;
            Vector2 newOffset = _offset;

            Vector2 newPivot = Vector2.zero;

            if (newPos.x + _offset.x + _child.rect.width >= Screen.width)
            {
                newPivot.x = 1;
                newOffset.x = -_offset.x;
            }

            if (newPos.y + _offset.y + _child.rect.height >= Screen.height)
            {
                newPivot.y = 1;
                newOffset.y = -_offset.y;
            }

            newPos += (Vector3)newOffset;

            _child.pivot = newPivot;
            _child.position = newPos;
        }

        private void ShowToolTip()
        {
            _tooltipIsShown = true;
            s_currentMessageUI.ShowTitle();
            _canvasGroup.alpha = 1;
        }

        private void ShowMessage()
        {
            _messageIsShown = true;
            s_currentMessageUI.ShowMessage();
        }
        
        /// <summary>
        /// Sub a ToolTip Messenger to the ToolTip.
        /// </summary>
        public static void Sub(ITooltipMessenger messenger) 
        {
            if (messenger == null || s_currentMessenger != null && messenger == s_currentMessenger)
                return;

            CloseToolTip();
            s_currentMessenger = messenger;
            s_currentMessageUI = GetMessageUI(messenger.tooltipMessage);
            OpenToolTip();
        }

        public static void Unsub(ITooltipMessenger messenger)
        {
            if (messenger == null || s_currentMessenger != messenger) return;
            
            s_currentMessenger = null;
            CloseToolTip();
        }

        private static TooltipMessageUI GetMessageUI( TooltipMessageUI messageIUTemplate)
        {
            if (messageIUTemplate == null) return messageIUTemplate;

            foreach (var messageUI in Instance._toolTipMessageUIs)
            {
                if (messageUI.template == messageIUTemplate)
                {
                    return messageUI;
                }
            }

            if (!Instantiate(messageIUTemplate.gameObject, Instance._messageLayout)
                    .TryGetComponent(out TooltipMessageUI message)) return null;

            message.template = messageIUTemplate;
            Instance._toolTipMessageUIs.Add(message);

            return message;
        }
    }
}