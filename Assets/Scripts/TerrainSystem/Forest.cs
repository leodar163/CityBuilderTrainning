using System.Collections.Generic;
using ResourceSystem;

namespace TerrainSystem
{
    public class Forest : TerrainType
    {
        public override List<ResourceDelta> GetResourceDelta()
        {
            return new List<ResourceDelta>
            {
                new(ResourceSet.Default.GetResource("resource_environment"), -1, 0,0)
            };
        }
    }
}