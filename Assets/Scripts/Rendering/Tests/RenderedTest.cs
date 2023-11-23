using UnityEngine;

namespace Rendering.Tests
{
    public class RenderedTest : IBatchRendered
    {
        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _scale;

        public IBatchRendered RenderingSelf => this;
        public InstanceRenderData RenderData { get; set; }

        Vector3 IBatchRendered.Position
        {
            get => _position;
            set => _position = value;
        }

        Quaternion IBatchRendered.Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        Vector3 IBatchRendered.Scale
        {
            get => _scale;
            set => _scale = value;
        }

        public RenderedTest(InstanceRenderData renderData)
        {
            RenderData = renderData;
            RenderingSelf.OnCreated();
        }

        public RenderedTest(InstanceRenderData renderData, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            RenderData = renderData;
            _position = position;
            _rotation = rotation;
            _scale = scale;
            RenderingSelf.OnCreated();
        }

        public void OnDestroy()
        {
            RenderingSelf.OnDestroyed();
        }
    }
}