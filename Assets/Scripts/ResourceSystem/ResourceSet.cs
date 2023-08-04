using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResourceSystem
{
    [CreateAssetMenu(menuName = "Resources/Resource Set", fileName = "NewResourceSet")]
    public class ResourceSet : ScriptableObject
    {
        [SerializeField] private List<ResourceType> _resources = new();

        private static ResourceSet s_default;

        public  static ResourceSet Default
        {
            get
            {
                if (s_default == null) s_default = Resources.Load("ResourceSetDefault.asset", typeof(ResourceSet)) as ResourceSet;
                if (s_default == null) throw new NullReferenceException($"No {nameof(ResourceSet)} with name 'ResourceSetDefault'" +
                                                                        $"found in a 'Resources' folder");
                return s_default;
            }
        }

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