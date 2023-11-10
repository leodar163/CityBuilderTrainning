using System;
using System.Collections.Generic;
using Effects;
using UnityEngine;
using UnityEngine.Localization;

namespace Events
{
    [CreateAssetMenu(menuName = "Events/Game Event", fileName = "NewGameEvent")]
    public class GameEvent : ScriptableObject
    {
        public Sprite icon;
        public Sprite illustration;
        [SerializeField] private LocalizedString _title;
        [SerializeField] private LocalizedString _summary;
        [SerializeField] private LocalizedString _desc;
        [Space] 
        [SerializeField] private ScriptableEffect[] _effects;
        public static event Action<GameEvent> onEventFired;
        
        public IEnumerable<ScriptableEffect> Effects => _effects;
        public string Title => _title.GetLocalizedString();
        public string Summary => _summary.IsEmpty ? "no_summary" : _summary.GetLocalizedString();
        public string Desc => _desc.IsEmpty ? "no_description" :_desc.GetLocalizedString();
 
        public void Fire()
        {
            foreach (var effect in _effects)
            {
                effect.GetEffectCopy().Apply();
            }
            
            onEventFired?.Invoke(this);
        }
    }
}