using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Tests
{
    public class BatchTest : MonoBehaviour
    {
        [SerializeField] private InstanceRenderData[] _renderDatas;
        
        private readonly List<RenderedTest> _renderedTests = new List<RenderedTest>();

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.P))
            {
                foreach (var renderData in _renderDatas)
                {
                    _renderedTests.Add(new RenderedTest(renderData, new Vector3()
                    {
                        x = Random.Range(-4,4),
                        z = Random.Range(-4,4)
                    }, Quaternion.Euler(new Vector3()
                    {
                        y = Random.Range(-180, 180)
                    }), Vector3.one));
                }
            }

            if (Input.GetKeyUp(KeyCode.M))
            {
                if (_renderedTests.Count < _renderDatas.Length) return;
                
                for (int i = 0; i < _renderDatas.Length; i++)
                {
                    _renderedTests[^1].OnDestroy();
                    _renderedTests.Remove(_renderedTests[^1]);
                }
            }
        }
    }
}