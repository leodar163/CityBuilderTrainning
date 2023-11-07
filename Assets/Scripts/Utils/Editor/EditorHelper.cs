using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    public static class EditorHelper
    {
        public static void TightLabelLayout(string labelStr)
        {
            GUIContent label = new GUIContent(labelStr);
            //This is the important bit, we set the width to the calculated width of the content in the GUIStyle of the control
            EditorGUILayout.LabelField(label, GUILayout.Width(GUI.skin.label.CalcSize(label).x));
        }
    }
}