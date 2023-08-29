using System.Collections.Generic;

namespace ResourceSystem
{
    public interface IResourceModifier
    {
        public string modifierName { get; }
        public List<ResourceDelta> GetResourceDelta();
    }
}