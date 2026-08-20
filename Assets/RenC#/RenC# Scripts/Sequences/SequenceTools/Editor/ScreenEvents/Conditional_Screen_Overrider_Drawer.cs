#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
namespace RenCSharp.Sequences.Editor
{
    [CustomPropertyDrawer(typeof(Conditional_Screen_Overrider))]
    public class Conditional_Screen_Overrider_Drawer : Screen_Event_Drawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Rect dDownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            DropDownMenu(dDownRect, property);
            EditorGUI.BeginChangeCheck();
            Rect newPos = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height);
            EditorGUI.PropertyField(newPos, property, new GUIContent(property.type), true);
            EditorGUI.EndChangeCheck();
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Drawer_Helper.PropertyHeight(property) + (property.Copy().CountInProperty() * EditorGUIUtility.singleLineHeight);
        }
    }
}
#endif