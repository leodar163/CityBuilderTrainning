using System;
using System.Collections.Generic;
using ResourceSystem.Categories;
using UnityEngine;
using Utils;

namespace ResourceSystem
{
    [CreateAssetMenu(menuName = "Resources/Resource Set", fileName = "NewResourceSet")]
    public class ResourceSet : DefaultableScriptableObject<ResourceSet>
    {
        [SerializeField] private List<ResourceType> _resources = new();

        public List<ResourceType> resources => _resources;

        private readonly Dictionary<ResourceCategory, List<ResourceType>> _resourcesByCategories = new(); 

        public ResourceType GetResource(string id)
        {
            foreach (var resource in _resources)
            {
                if (resource.id == id)
                    return resource;
            }

            return null;
        }

        public List<ResourceType> GetResourcesByCategory(ResourceCategory category)
        {
            if (_resourcesByCategories.TryAdd(category, new List<ResourceType>()))
            {
                foreach (var resource in _resources)
                {
                    if (resource.Category != null && resource.Category == category)
                        _resourcesByCategories[category].Add(resource);
                }
            }

            return new List<ResourceType>(_resourcesByCategories[category]);
        }
    }
}