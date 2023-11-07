using UnityEditor;
using UnityEngine;

namespace Events.Editor
{
    [CustomPropertyDrawer(typeof(EventTimeRange))]
    public class EventTimeRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty gameEvent = property.FindPropertyRelative("gameEvent");
            SerializedProperty minDate = property.FindPropertyRelative("minDate");
            SerializedProperty maxDate = property.FindPropertyRelative("maxDate");

            
            EditorGUILayout.PrefixLabel(property.displayName);
            EditorGUILayout.PropertyField(gameEvent);
            EditorGUILayout.Space(-20);
            EditorGUILayout.PropertyField(minDate);
            EditorGUILayout.Space(-20);
            EditorGUILayout.PropertyField(maxDate);
        }
    }
}