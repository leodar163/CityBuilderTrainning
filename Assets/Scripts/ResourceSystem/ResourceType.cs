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
        public string resourceName { get; private set; }
        public string description => _desc.IsEmpty ? "no_description" : _desc.GetLocalizedString();

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
            resourceName = _name.GetLocalizedString();
        }
    }
}