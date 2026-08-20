#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
namespace RenCSharp.Sequences.Editor
{
    [CustomPropertyDrawer(typeof(Player_Button_Flag))]
    public class PlayerButton_Drawer : Screen_Event_Drawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Rect dDownRect = new (position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            DropDownMenu(dDownRect, property);
            Rect newRect = new (position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height);
            EditorGUI.PropertyField(newRect, property, new GUIContent("Player Button Flag"), true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Drawer_Helper.PropertyHeight(property) + (7 * EditorGUIUtility.singleLineHeight);
        }
    }
}
#endif