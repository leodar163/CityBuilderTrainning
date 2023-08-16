using TMPro;
using UnityEngine;
using Utils;
using Utils.UI;

namespace ToolTipSystem
{
    public class ToolTip : Singleton<ToolTip>
    {
        [SerializeField] private RectTransform _child; 
        [SerializeField] private Vector2 _offset = new (10,0);
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Texts")] 
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _message;

        private Camera _mainCamera;

        private static IToolTipSpeaker _currentSpeaker;
        private static IToolTipSpeaker _nextSpeaker;

        [Header("Timers")]
        public float timeToShowToolTip = 0.1f;
        public float timeToShowMessage = 0.5f;

        private float _timeBeingOpened;

        private static bool _tooltipIsShown;
        private static bool _messageIsShown;

        private static bool isOpen;
        
        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_nextSpeaker == null)
            {
                CloseToolTip();
            }
            else if (_currentSpeaker == null || _nextSpeaker != _currentSpeaker)
            {           
                CloseToolTip();
                OpenToolTip();
            }

            _currentSpeaker = _nextSpeaker;
            _nextSpeaker = null;
            
            if (isOpen)
            {
                _timeBeingOpened += Time.deltaTime;

                UpdateContent();
                
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
            Instance._message.gameObject.SetActive(false);
        }

        private static void OpenToolTip()
        {
            isOpen = true;
        }


        private static void UpdateContent()
        {
            ToolTipMessage message = _currentSpeaker.ToToolTipMessage();
            
            Instance._title.text = message.title;
            Instance._message.text = message.message;
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
            _canvasGroup.alpha = 1;
        }

        private void ShowMessage()
        {
            _messageIsShown = true;
            _message.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Sub a ToolTip speaker to the ToolTip. Need to sub every frame to continue showing up the ToolTip. 
        /// </summary>
        public static void Sub(IToolTipSpeaker speaker)
        {
            _nextSpeaker = speaker;
        }
    }
}