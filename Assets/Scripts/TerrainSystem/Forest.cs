using System.Collections.Generic;
using ResourceSystem;

namespace TerrainSystem
{
    public class Forest : TerrainType
    {
        public override List<ResourceQuantity> GetResourceDelta()
        {
            return new List<ResourceQuantity>
            {
                new(ResourceSet.Default.GetResource("resource_environment"), -1)
            };
        }
    }
}