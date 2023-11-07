using UnityEditor;
using UnityEngine;
using Utils.Editor;

namespace TimeSystem.Editor
{
    [CustomPropertyDrawer(typeof(InGameDate), true)]
    public class InGameDateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty totalMonths = property.FindPropertyRelative("totalMonths");

            //EditorGUILayout.PropertyField(totalMonths);
            
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel(property.displayName);
            int yearCount = totalMonths.intValue / 12;
            int monthCount = totalMonths.intValue % 12;
            
            EditorHelper.TightLabelLayout("year");
            yearCount = EditorGUILayout.IntField(yearCount);
            EditorHelper.TightLabelLayout("month");
            monthCount = EditorGUILayout.IntField(monthCount);
            
            EditorGUILayout.EndHorizontal();
            totalMonths.intValue = yearCount * 12 + monthCount;
            
            EditorGUI.EndProperty();
        }
    }
}