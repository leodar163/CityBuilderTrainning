using UnityEngine;

namespace Rendering
{
    public interface IBatchRendered
    {
        public IBatchRendered BatchRenderedSelf { get; }
        
        public InstanceRenderData RenderData { get; }
        public Vector3 Position { get; protected set; }
        public Quaternion Rotation { get; protected set; }
        public Vector3 Scale { get; protected set; }

        public void SetPosition(Vector3 newPosition)
        {
            Position = newPosition;
            BatchRenderer.NotifyMatrixChanges(this);
        }

        public void SetRotation(Quaternion newRotation)
        {
            Rotation = newRotation;
            BatchRenderer.NotifyMatrixChanges(this);
        }

        public void SetScale(Vector3 newScale)
        {
            Scale = newScale;
            BatchRenderer.NotifyMatrixChanges(this);
        }

        public void SetTransform(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            Position = pos;
            Rotation = rot;
            Scale = scale;
            BatchRenderer.NotifyMatrixChanges(this);
        }
        
        public Matrix4x4 GetMatrix()
        {
            return Matrix4x4.TRS(Position, Rotation, Scale);
        }

        public void OnCreated()
        {
            BatchRenderer.AddBatchRendered(this);
        }

        public void OnDestroyed()
        {
            BatchRenderer.RemoveBatchRendered(this);
        }
    }
}