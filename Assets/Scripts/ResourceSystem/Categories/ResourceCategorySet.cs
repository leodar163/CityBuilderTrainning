using UnityEngine;
using Utils;

namespace ResourceSystem.Categories
{
    [CreateAssetMenu(menuName = "Resources/Categories/Category Set", fileName = "NewResourceCategorySet")]
    public class ResourceCategorySet: DefaultableScriptableObject<ResourceCategorySet>
    {
        [SerializeField] private ResourceCategory[] _categories;
        public ResourceCategory[] Categories => _categories;
        
        public ResourceCategory GetCategory(string id)
        {
            foreach (var category in _categories)
            {
                if (category.id == id)
                    return category;
            }

            return null;
        }
    }
}