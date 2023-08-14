using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace ResourceSystem
{
    [CreateAssetMenu(menuName = "Resources/Resource Set", fileName = "NewResourceSet")]
    public class ResourceSet : DefaultableScriptableObject<ResourceSet>
    {
        [SerializeField] private List<ResourceType> _resources = new();

        public ResourceType GetResource(string id)
        {
            foreach (var resource in _resources)
            {
                if (resource.id == id)
                    return resource;
            }

            return null;
        }
    }
}