using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Localization
{
    [Serializable]
    public class AutoLocalizedString
    {
        public static readonly List<AutoLocalizedString> allAutoLocalizedStrings = new();

        [SerializeField] private LocalizedString _localizedString;
        private string _string = "no_string";

        public string String => _string;

        public event Action<AutoLocalizedString> onStringChanged;

        public AutoLocalizedString()
        {
            allAutoLocalizedStrings.Add(this);
        }
        
        public void Init()
        {
            if (_localizedString is { IsEmpty: false }) 
                _string = _localizedString.GetLocalizedString();
            
            LocalizationSettings.SelectedLocaleChanged += UpdateString;
        }
        
        private void UpdateString(Locale locale = null)
        {
            _string = _localizedString.GetLocalizedString();
            onStringChanged?.Invoke(this);
        }
    }
}