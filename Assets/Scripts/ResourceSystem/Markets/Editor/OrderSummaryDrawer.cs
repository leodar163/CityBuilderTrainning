using UnityEditor;
using UnityEngine;

namespace ResourceSystem.Markets.Editor
{
    [CustomPropertyDrawer(typeof(OrderSummary))]
    public class OrderSummaryDrawer : PropertyDrawer 
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty resource = property.FindPropertyRelative("resource");
            SerializedProperty quantity = property.FindPropertyRelative("quantity");
            SerializedProperty orderType = property.FindPropertyRelative("orderType");

            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.PrefixLabel(property.displayName);
            EditorGUILayout.PropertyField(resource, GUIContent.none);
            EditorGUILayout.PropertyField(quantity, GUIContent.none);
            EditorGUILayout.PropertyField(orderType, GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
    }
}