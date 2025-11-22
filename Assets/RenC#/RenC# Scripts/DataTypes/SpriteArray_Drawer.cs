#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RenCSharp
{
    /// <summary>
    /// MAY OR MAY NOT BE GROSS IF YOUR INSPEcTOR WINDOW IS ScALED SLIGHTLY INcONVENIENTLY!
    /// </summary>
    [CustomPropertyDrawer(typeof(SpriteArray))]
    public sealed class SpriteArray_Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty spritesArray = property.FindPropertyRelative("layer");
            SerializedProperty stringsArray = property.FindPropertyRelative("visualIDs");

            int spASize = spritesArray.arraySize;
            int stASize = stringsArray.arraySize;

            Rect leftR = new Rect(position.x, position.y, position.width / 2.25f, position.height);
            Rect rightR = new Rect(position.x + (position.width / 2), position.y, position.width / 2.25f, position.height);
            GUIContent leftLab = new GUIContent("Sprites");
            GUIContent rightLab = new GUIContent("IDs");

            EditorGUI.PropertyField(leftR, spritesArray, leftLab, true);
            EditorGUI.PropertyField(rightR, stringsArray, rightLab, true);

            //EditorGUI.PropertyField(position, property, label, true); <- default behavior
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty spritesArray = property.FindPropertyRelative("layer");
            SerializedProperty stringsArray = property.FindPropertyRelative("visualIDs");

            int spArrayLength = spritesArray.arraySize;
            int stArrayLength = stringsArray.arraySize;

            float returner = Drawer_Helper.PropertyHeight((spArrayLength > stArrayLength) ? spritesArray : stringsArray);
            returner += EditorGUIUtility.singleLineHeight * 3 + (Mathf.Max(spArrayLength, stArrayLength) * 2);
            return returner;
        }
    }
}
#endif