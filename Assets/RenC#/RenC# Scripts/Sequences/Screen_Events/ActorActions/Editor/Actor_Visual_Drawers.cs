#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using RenCSharp.Actors;
namespace RenCSharp.Sequences
{
    [CustomPropertyDrawer(typeof(Spawn_Actor))]
    public class Spawn_Actor_Drawer : Screen_Event_Drawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            m_SE = property;
            Rect dDownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            DropDownMenu(dDownRect, property);
            Rect newR = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height);
            EditorGUI.PropertyField(newR, property, new GUIContent(property.type + " DON'T OPEN ME"), true);
            
            SerializedProperty spawnOffset = property.FindPropertyRelative("spawnOffset");
            SerializedProperty fadeInTime = property.FindPropertyRelative("fadeInTime");
            SerializedProperty sprindexArray = property.FindPropertyRelative("visualSpriteIndexes");
            SerializedProperty actorProperty = property.FindPropertyRelative("actorToSpawn");
            Actor assignedActor = actorProperty.objectReferenceValue as Actor;

            Rect actRect = new Rect(newR.x, newR.y + (EditorGUIUtility.singleLineHeight * 1), newR.width, EditorGUIUtility.singleLineHeight * 2);
            Rect spawnRect = new Rect(newR.x, newR.y + (EditorGUIUtility.singleLineHeight * 2.5f), newR.width, EditorGUIUtility.singleLineHeight * 2);
            Rect fadeInRect = new Rect(newR.x, newR.y + (EditorGUIUtility.singleLineHeight * 4), newR.width, EditorGUIUtility.singleLineHeight * 2);
            Rect sprinRect = new Rect(newR.x, newR.y + (EditorGUIUtility.singleLineHeight * 5.5f), newR.width, EditorGUIUtility.singleLineHeight * 2);

            EditorGUI.PropertyField(actRect, actorProperty, new GUIContent("Actor to Spawn"), true);
            EditorGUI.PropertyField(spawnRect, spawnOffset, new GUIContent("SpawnOffset"), true);
            EditorGUI.PropertyField(fadeInRect, fadeInTime, new GUIContent("FadeInTime"), true);
            EditorGUI.PropertyField(sprinRect, sprindexArray, new GUIContent("Visual Sprite Indexes"), true);
            //Debug.Log("Assigned actor: " + assignedActor.ActorName);
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Drawer_Helper.PropertyHeight(property) + EditorGUIUtility.singleLineHeight * 10;
        }
    }
    [CustomPropertyDrawer(typeof(Actor_Expression))]
    public class Actor_Expression_Drawer : Screen_Event_Drawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            m_SE = property;
            Rect dDownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            DropDownMenu(dDownRect, property);
            Rect newR = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height);
            EditorGUI.PropertyField(newR, property, new GUIContent(property.type + "DONT OPEN ME"), true);

            SerializedProperty actorProperty = property.FindPropertyRelative("actorToAlter");
            SerializedProperty sprindexArray = property.FindPropertyRelative("visualSpriteIndexes");
            Actor actorToAlter = actorProperty.objectReferenceValue as Actor;
            //if(actorToAlter != null) Debug.Log(actorToAlter.ActorName);
            Rect actRect = new Rect(newR.x, newR.y + EditorGUIUtility.singleLineHeight, newR.width, EditorGUIUtility.singleLineHeight * 2);
            Rect sprinRect = new Rect(newR.x, newR.y + (EditorGUIUtility.singleLineHeight * 2.5f), newR.width, EditorGUIUtility.singleLineHeight * 2);

            EditorGUI.PropertyField(actRect, actorProperty, new GUIContent("Actor to Alter"), true);
            EditorGUI.PropertyField(sprinRect, sprindexArray, new GUIContent("Visual Sprite Indexes"), true);

            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Drawer_Helper.PropertyHeight(property) + EditorGUIUtility.singleLineHeight * 10;
        }
    }
}
#endif