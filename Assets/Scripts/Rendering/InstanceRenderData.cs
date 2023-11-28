using UnityEngine;
using UnityEngine.Rendering;

namespace Rendering
{
    [CreateAssetMenu(menuName = "Batch Rendering/Render data", fileName = "NewRenderingData")]
    public class InstanceRenderData : ScriptableObject
    {
        public Mesh mesh;
        public Material material;

        private RenderParams _renderParams;

        public RenderParams GetRenderParams()
        {
            if (_renderParams.material == null)
            {
                _renderParams = new RenderParams(material)
                {
                    shadowCastingMode = ShadowCastingMode.On
                };
            }

            return _renderParams;
        }
    }
}