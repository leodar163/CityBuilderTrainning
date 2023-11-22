using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class GUPInstancingTest : MonoBehaviour
{
    [SerializeField] [Min(0)] private int _instanceCount;
    
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    [SerializeField] private float3 _rotation;
    [SerializeField] private float3 _scale;

    private NativeArray<float3> _nativePositions;
    private NativeArray<Matrix4x4> _nativeMatrices;
    private RenderData[] _renderDatas;
    private NativeArray<NativeArray<Matrix4x4>> _nativeMatricesBatches;

    private MeshPositionJob _positionJob;

    private RenderParams _renderParams;

    [SerializeField] private RenderBatchData[] _batches;
    
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
           
        }
    }

    private void OnDisable()
    {
        _nativePositions.Dispose();
        _nativeMatrices.Dispose();

        foreach (var batch in _batches)
        {
            batch.Dispose();
        }

        foreach (var batch in _nativeMatricesBatches)
        {
            batch.Dispose();
        }

        _nativeMatricesBatches.Dispose();
    }

    private void Start()
    {
        InitTest2();
    }

    private void Update()
    {
        RunTest2();
    }

    #region test1

    private void ReinitTest1()
    {
        if (_nativePositions.IsCreated)
            _nativePositions.Dispose();
        
        _nativePositions = new NativeArray<float3>(_instanceCount, Allocator.Persistent);
        
        if (_nativeMatrices.IsCreated)
            _nativeMatrices.Dispose();
        
        _nativeMatrices = new NativeArray<Matrix4x4>(_instanceCount, Allocator.Persistent);

        for (int i = 0; i < _instanceCount; i++)
        {
            _nativePositions[i] = new float3(i,0,i);
        }

        _positionJob = new MeshPositionJob
        {
            positions = _nativePositions,
            matrices = _nativeMatrices,
            rotation = _rotation,
            scale = _scale
        };

        _renderParams = new RenderParams(_material);
    }

    private void RunTest1()
    {
        _positionJob.matrices = _nativeMatrices;
        _positionJob.time += Time.deltaTime;
        _positionJob.Schedule(_instanceCount, 64).Complete();
        
        Graphics.RenderMeshInstanced(_renderParams, _mesh,0, _nativeMatrices);
    }

    [BurstCompile]
    internal struct MeshPositionJob : IJobParallelFor
    {
        public NativeArray<float3> positions;
        public NativeArray<Matrix4x4> matrices;
        public float3 rotation;
        public float3 scale;
        public float time;

        public void Execute(int index)
        {
            positions[index] = new float3(index % 40,time % 3,index / 40);
            matrices[index] = Matrix4x4.TRS(positions[index], Quaternion.Euler(rotation), scale);
        }
    }
    
    #endregion

    #region Test2

    private void InitTest2()
    {
        _nativeMatricesBatches = new NativeArray<NativeArray<Matrix4x4>>(_batches.Length, Allocator.Persistent);
        _renderDatas = new RenderData[_batches.Length];

        for (int i = 0; i < _batches.Length; i++)
        {
            _nativeMatricesBatches[i] = _batches[i].GetMatrices();
            _renderDatas[i] = _batches[i].renderData;
        }
    }
    
    private void RunTest2()
    {
        for (int i = 0; i < _batches.Length; i++)
        {
            Graphics.RenderMeshInstanced( _renderDatas[i].RenderParams, _renderDatas[i].mesh,0 ,_nativeMatricesBatches[i]);
        }
    }

    [Serializable]
    public struct RenderData
    {
        public Mesh mesh;
        public Material material;

        public RenderParams RenderParams => new RenderParams(material);
    }

    [Serializable]
    public struct SimpleMatrix
    {
        public float3 position;
        public float3 rotation;
        public float3 scale;

        public Matrix4x4 Matrix => Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale);
    }

    [Serializable]
    public struct RenderBatchData
    {
        public RenderData renderData;
        public SimpleMatrix[] matricesSummaries;
        private NativeArray<Matrix4x4> matrices; 

        public NativeArray<Matrix4x4> GetMatrices()
        {
            if (matrices.IsCreated) return matrices;
            
            matrices = new NativeArray<Matrix4x4>(matricesSummaries.Length, Allocator.Persistent);
                
            for (int i = 0; i < matricesSummaries.Length; i++)
            {
                matrices[i] = matricesSummaries[i].Matrix;
            }
                
            return matrices;
        }

        public void Dispose()
        {
            matrices.Dispose();
        }
    }

    #endregion
}
