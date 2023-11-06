using ResourceSystem.Categories;
using UnityEngine;
using UnityEngine.Localization;

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
        public string resourceName => _name.GetLocalizedStringAsync().Result;
        public string description => _desc.IsEmpty ? "no_description" : _desc.GetLocalizedString();
    }
}