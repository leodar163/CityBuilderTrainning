using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ToolTipSystem
{
    public class ToolTipMessengerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private MonoBehaviour _message;
        private IToolTipSpeaker toolTipSpeaker;
        private bool _mouseIsIn;

        private void OnValidate()
        {
            if (_message && _message is not IToolTipSpeaker)
                _message = null;
        }

        private void Awake()
        {
            if (_message)
                toolTipSpeaker = (IToolTipSpeaker)_message;
        }

        private void Update()
        {
            if (toolTipSpeaker != null && _mouseIsIn)
            {
                ToolTip.Sub(toolTipSpeaker);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseIsIn = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseIsIn = false;
        }
    }
}