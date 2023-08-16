using UnityEngine;

namespace ToolTipSystem
{
    public abstract class ToolTipMessengerBase : MonoBehaviour
    {
        [Tooltip("Must be IToolTipMessage derived")]
        [SerializeField] protected MonoBehaviour _message;
        protected IToolTipSpeaker ToolTipSpeaker;

        protected virtual void OnValidate()
        {
            if (_message && ToolTipSpeaker == null)
            {
                ToolTipSpeaker = _message as IToolTipSpeaker;
                
                if(ToolTipSpeaker == null)
                    _message = null;
            }

            if (ToolTipSpeaker != null && _message == null)
            {
                ToolTipSpeaker = null;
            }
        }
    }
}