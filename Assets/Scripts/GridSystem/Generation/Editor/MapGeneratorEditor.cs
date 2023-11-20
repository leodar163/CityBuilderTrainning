using UnityEditor;
using UnityEngine;

namespace GridSystem.Generation.Editor
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            if (GUILayout.Button("Generate Map"))
            {
                (target as MapGenerator)?.GenerateMap(false, true);
            }

            GUILayout.Label((target as MapGenerator)?.noiseMap);
        }
        
    }

}