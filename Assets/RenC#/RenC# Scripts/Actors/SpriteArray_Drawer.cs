#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RenCSharp
{
    [CustomPropertyDrawer(typeof(SpriteArray))]
    public class SpriteArray_Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty spritesArray = property.FindPropertyRelative("layer");
            SerializedProperty stringsArray = property.FindPropertyRelative("visualIDs");

            //Debug.Log("Sparray: " + spritesArray.arraySize);
            //Debug.Log("Strarray: " + stringsArray.arraySize);

            Rect leftR = new Rect(position.x, position.y, position.width / 2, position.height);
            Rect rightR = new Rect(position.x + (position.width / 2), position.y, position.width / 2, position.height);
            GUIContent leftLab = new GUIContent("Layer");
            GUIContent rightLab = new GUIContent("IDs");

            EditorGUI.PropertyField(leftR, spritesArray, leftLab, true);
            EditorGUI.PropertyField(rightR, stringsArray, rightLab, true);

            //EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float returner = Drawer_Helper.PropertyHeight(property);
            returner += EditorGUIUtility.singleLineHeight * 3;
            return returner;
        }
    }
}
#endif