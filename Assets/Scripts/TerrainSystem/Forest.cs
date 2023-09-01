using System.Collections.Generic;
using ResourceSystem;

namespace TerrainSystem
{
    public class Forest : TerrainType
    {
        public override ResourceDelta[] GetResourceDelta()
        {
            return new ResourceDelta[]
            {
                new(ResourceSet.Default.GetResource("resource_environment"), -1)
            };
        }
    }
}