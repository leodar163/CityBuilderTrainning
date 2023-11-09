using System;
using Effects;
using UnityEngine;
using UnityEngine.Localization;

namespace Events
{
    public class GameEvent : ScriptableObject
    {
        public Sprite illustration;
        [SerializeField] private LocalizedString _title;
        [SerializeField] private LocalizedString _desc;
        [Space] 
        [SerializeField] private Effect[] _effects;
        public static event Action<GameEvent> onEventFired;

        public Effect[] Effects => _effects;
        public string Title => _title.GetLocalizedString();
        public string Desc => _desc.GetLocalizedString();

        public void Fire()
        {
            foreach (var effect in _effects)
            {
                effect.Apply();
            }
            
            onEventFired?.Invoke(this);
        }
    }
}