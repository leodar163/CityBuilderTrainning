using TMPro;
using UnityEngine;

namespace ToolTipSystem.Messages
{
    public class TextTooltipMessageUI : TooltipMessageUI
    {
        [Header("Texts")] 
        [SerializeField] private TextMeshProUGUI _textTitle;
        [SerializeField] private TextMeshProUGUI _textMessage;
            
        public void SetTexts(string title, string message)
        {
            _textTitle.SetText(title);
            _textMessage.SetText(message);
        }
    }
}