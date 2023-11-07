using UnityEngine;
using UnityEngine.Localization;

namespace Events
{
    public class GameEvent : ScriptableObject
    {
        public Sprite illustration;
        [SerializeField] private LocalizedString _title;
        [SerializeField] private LocalizedString _desc;

        public string Title => _title.GetLocalizedString();
        public string Desc => _desc.GetLocalizedString();
    }
}