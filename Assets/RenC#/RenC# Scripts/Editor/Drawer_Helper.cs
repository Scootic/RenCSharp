#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace RenCSharp
{
    /// <summary>
    /// Used by custom property drawers to make sure that there's no repeated useful functions lying around in multiple scripts.
    /// </summary>
    public static class Drawer_Helper
    {
        /// <summary>
        /// Should automatically make a property readable and usable. Sets how big it's box appears in Editor.
        /// </summary>
        /// <param name="property">The class, or whoever, Editor is trying to display.</param>
        /// <returns>How big the box will be in Editor.</returns>
        public static float PropertyHeight(SerializedProperty property)
        {
            float returner = EditorGUIUtility.singleLineHeight + (EditorGUIUtility.standardVerticalSpacing * 2);
            SerializedProperty copy = property.Copy();
            if (copy.isExpanded)
            {
                int childc = copy.CountInProperty();
                returner *= childc;
            }
            return returner;
        }
        /// <summary>
        /// Get a rect to place your auto-fill string box in. 
        /// </summary>
        /// <param name="startingRect">The starting rect you want to alter.</param>
        /// <param name="iteration">Which iteration you are on.</param>
        /// <returns>The new Rect.</returns>
        public static Rect AutoFillTextBox(Rect startingRect, int iteration)
        {
            float yPos = startingRect.y + EditorGUIUtility.standardVerticalSpacing + 
                (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2) * (iteration + 1);
            Rect toReturn = new Rect(startingRect.x, yPos, startingRect.width, startingRect.height);
            return toReturn;
        }
    }
}
#endif