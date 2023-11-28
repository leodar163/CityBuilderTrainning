using System;
using ResourceSystem.Categories;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace ResourceSystem
{
    [CreateAssetMenu(menuName = "Resources/Resource", fileName = "NewResource")]
    public class ResourceType : ScriptableObject
    {
        [SerializeField] private ResourceCategory _category;
        [SerializeField] private string _id;
        [SerializeField] private LocalizedString _name;
        [SerializeField] private LocalizedString _desc;
        public Sprite icon;
        public ResourceCategory Category => _category;
        public string id => _id;

        string _resourceName;
        
        public string ResourceName
        {
            get
            {
                if (_resourceName == "") InitName();
                return _resourceName;
            }
        }

        private string _description;
        
        public string Description
        {
            get
            {
                if (_description == "") InitName();
                return _description;
            }
        }

        private void Awake()
        {
            InitName();
        }

        private void OnEnable()
        {
            LocalizationSettings.SelectedLocaleChanged += InitName;
        }

        private void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= InitName;
        }

        private void InitName(Locale locale = null)
        {
            _resourceName = _name.GetLocalizedString();
            _description = _desc.IsEmpty ? "no_description" : _desc.GetLocalizedString();
        }
    }
}