using TimeSystem;
using UnityEngine;
using UnityEngine.Localization;

namespace Effects
{
    public abstract class Effect : ScriptableObject
    {
        [SerializeField] private Sprite _icon;
        [Space]
        [SerializeField] private LocalizedString _name; 
        [SerializeField] private LocalizedString _format;
        
        [Tooltip("0 means it will last forever")]
        [SerializeField] [Min(0)] private int _duration;

        public Sprite Icon => _icon;
        public string EffectName => _name.GetLocalizedString();
        public string Format => FormatMessage();

        protected virtual string FormatMessage()
        {
            return _format.GetLocalizedString();
        }

        public virtual void Apply()
        {
            if (_duration > 0) new Timer(_duration, UpdateMoment.OnNewMonth).StartTimer().onTimerFinished += Unapply;
        }

        public virtual void Unapply()
        {
            
        }
    }
}