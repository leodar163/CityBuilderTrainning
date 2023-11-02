using UnityEngine;
using UnityEngine.Localization;

namespace ResourceSystem.Categories
{
    [CreateAssetMenu(menuName = "Resources/Categories/Category", fileName = "NewResourceCategory")]
    public class ResourceCategory : ScriptableObject
    {
        [SerializeField] private string _id;
        public string id => _id;
        
        [SerializeField] private LocalizedString _localizedName;
        public string categoryName => _localizedName.GetLocalizedString();
        
        public Sprite icon;
    }
}