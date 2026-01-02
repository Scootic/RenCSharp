using UnityEngine;
using UnityEditor;
namespace RenCSharp.Sequences
{
    [CustomPropertyDrawer(typeof(Conditional_Screen_Overrider))]
    public class Conditional_Screen_Overrider_Drawer : Screen_Event_Drawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Drawer_Helper.PropertyHeight(property) + EditorGUIUtility.singleLineHeight * 10f;
        }
    }
}
