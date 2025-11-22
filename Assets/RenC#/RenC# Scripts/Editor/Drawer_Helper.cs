#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
namespace RenCSharp
{
    public static class Drawer_Helper
    {
        public static float PropertyHeight(SerializedProperty property)
        {
            float returner = EditorGUIUtility.singleLineHeight;
            SerializedProperty copy = property.Copy();
            if (copy.isExpanded)
            {
                int childc = copy.CountInProperty();
                returner *= childc;
            }
            return returner;
        }
    }
}
#endif