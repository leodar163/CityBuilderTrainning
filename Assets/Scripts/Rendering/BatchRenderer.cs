using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Rendering
{
    public class BatchRenderer : Singleton<BatchRenderer>
    {
        private static readonly Dictionary<IBatchRendered, int> s_batchRendereds = new();
        private static readonly Dictionary<InstanceRenderData, Matrix4x4[]> s_batches = new();

        public static void AddBatchRendered(IBatchRendered batchRendered)
        {
            if (s_batchRendereds.ContainsKey(batchRendered)) return;

            if (!s_batches.TryGetValue(batchRendered.RenderData, out var matrices))
            {
                matrices = new Matrix4x4[1023];
                s_batches.Add(batchRendered.RenderData, matrices);
            }

            for (int i = 0; i < matrices.Length; i++)
            {
                if (matrices[i] == default)
                {
                    matrices[i] = batchRendered.GetMatrix();
                    s_batchRendereds.Add(batchRendered, i);
                    return;
                }
            }

            throw new OutOfMemoryException(
                "Can only render 1023 instance of the same RenderData. Stop trying to render more");
        }

        public static void RemoveBatchRendered(IBatchRendered batchRendered)
        {
            if (!s_batchRendereds.ContainsKey(batchRendered)) return;

            s_batches[batchRendered.RenderData][s_batchRendereds[batchRendered]] = default;

            s_batchRendereds.Remove(batchRendered);

            if (s_batches[batchRendered.RenderData].Length == 0) s_batches.Remove(batchRendered.RenderData);
        }

        public static void NotifyMatrixChanges(IBatchRendered batchRendered)
        {
            if (!s_batchRendereds.ContainsKey(batchRendered)) return;
            s_batches[batchRendered.RenderData][s_batchRendereds[batchRendered]] = batchRendered.GetMatrix();
        }
        
        private void Update()
        {
            foreach (var batch in s_batches)
            {
                Graphics.RenderMeshInstanced(batch.Key.GetRenderParams(), batch.Key.mesh, 0, batch.Value);
            }
        }
    }
}