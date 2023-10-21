using UnityEngine;

namespace ToolTipSystem
{
    public abstract class TooltipMessageUI : MonoBehaviour
    {
        public TooltipMessageUI template;
        [Space] 
        [SerializeField] protected RectTransform _title;
        [SerializeField] protected RectTransform _message;

        public virtual void ShowTitle()
        {
            gameObject.SetActive(true);
            _title.gameObject.SetActive(true);
        }

        public virtual void ShowMessage()
        {
            _message.gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            _title.gameObject.SetActive(false);
            _message.gameObject.SetActive(false);
        }
    }
}