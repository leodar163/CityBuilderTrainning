using System;
using TimeSystem;
using UnityEngine;
using UnityEngine.Localization;

namespace Effects
{
    [Serializable]
    public abstract class Effect
    {
        [SerializeField] protected Sprite _icon;
        [Space]
        [SerializeField] protected LocalizedString _name;

        [Tooltip("0 means it will last forever")]
        [SerializeField] [Min(0)] protected int _duration;

        private bool _isApplied;

        private Timer _timer;
        
        public bool isApplied => _isApplied;

        public Sprite Icon => _icon;
        public string EffectName => _name.GetLocalizedString();

        public virtual string GetFormatMessage()
        {
            return "No_Format";
        }

        #region CONSTRUCTORS

        protected Effect(Sprite icon, LocalizedString name, int duration)
        {
            _icon = icon;
            _name = name;
            _duration = duration;
        }

        #endregion
        
        public virtual void Apply()
        {
            if (_duration > 0)
            {
                _timer = new Timer(_duration, UpdateMoment.OnNewMonth);
                _timer.StartTimer().onTimerFinished += Unapply;
            }
            _isApplied = true;
        }

        public virtual void Unapply()
        {
            _isApplied = false;
        }
    }
}