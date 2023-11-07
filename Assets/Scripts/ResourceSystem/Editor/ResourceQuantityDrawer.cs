using UnityEditor;
using UnityEngine;

namespace ResourceSystem.Editor
{
    [CustomPropertyDrawer(typeof(ResourceQuantity))]
    public class ResourceQuantityDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty resource = property.FindPropertyRelative("resource");
            SerializedProperty quantity = property.FindPropertyRelative("quantity");

            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.PrefixLabel(property.displayName);
            EditorGUILayout.PropertyField(resource, GUIContent.none);
            EditorGUILayout.PropertyField(quantity, GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
    }
}