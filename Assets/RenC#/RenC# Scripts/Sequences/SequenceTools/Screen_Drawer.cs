#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace RenCSharp.Sequences
{
    [CustomPropertyDrawer(typeof(Screen))]
    public class Screen_Drawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ScreenUITKField theField = new ScreenUITKField(property.name, (Screen)property.boxedValue);
            theField.SetScreenEventsProperty = property.FindPropertyRelative("ScreenActions");
            return theField;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int screenEventcount = property.FindPropertyRelative("ScreenActions").arraySize;
            return Drawer_Helper.PropertyHeight(property) + screenEventcount * EditorGUIUtility.singleLineHeight + EditorGUIUtility.singleLineHeight * 3;
        }
    }
}
#endif