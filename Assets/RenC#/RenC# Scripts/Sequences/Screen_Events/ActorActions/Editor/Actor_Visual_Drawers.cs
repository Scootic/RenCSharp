#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using RenCSharp.Actors;
namespace RenCSharp.Sequences
{
    [CustomPropertyDrawer(typeof(Spawn_Actor))]
    public class Spawn_Actor_Drawer : Screen_Event_Drawer
    {
        SerializedProperty spawnOffset = null;
        SerializedProperty fadeInTime;
        SerializedProperty sprindexArray;
        SerializedProperty actorProperty;
        Actor assignedActor;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            m_SE = property;
            Rect dDownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            DropDownMenu(dDownRect, property);
            Rect newR = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height);
            EditorGUI.PropertyField(newR, property, new GUIContent("Spawn Actor"), true);

            spawnOffset = property.FindPropertyRelative("spawnOffset");
            fadeInTime = property.FindPropertyRelative("fadeInTime");
            sprindexArray = property.FindPropertyRelative("visualSpriteIndexes");
            actorProperty = property.FindPropertyRelative("actorToSpawn");

            assignedActor = actorProperty?.objectReferenceValue as Actor;

            EditorGUI.BeginChangeCheck();

            Rect actRect = new Rect(newR.x, newR.y + (EditorGUIUtility.singleLineHeight * 1), newR.width, EditorGUIUtility.singleLineHeight * 2);
            Rect spawnRect = new Rect(newR.x, newR.y + (EditorGUIUtility.singleLineHeight * 2.5f), newR.width, EditorGUIUtility.singleLineHeight * 2);
            Rect fadeInRect = new Rect(newR.x, newR.y + (EditorGUIUtility.singleLineHeight * 4), newR.width, EditorGUIUtility.singleLineHeight * 2);
            Rect sprinRect = new Rect(newR.x, newR.y + (EditorGUIUtility.singleLineHeight * 5.5f), newR.width, EditorGUIUtility.singleLineHeight * 2);

            EditorGUI.PropertyField(actRect, actorProperty, new GUIContent("Actor to Spawn"), true);
            EditorGUI.PropertyField(spawnRect, spawnOffset, new GUIContent("SpawnOffset"), true);
            EditorGUI.PropertyField(fadeInRect, fadeInTime, new GUIContent("FadeInTime"), true);
            EditorGUI.PropertyField(sprinRect, sprindexArray, new GUIContent("Visual Sprite Indexes"), true);

             //Debug.Log("Assigned actor: " + assignedActor.ActorName);
            if (assignedActor != null)
            {
                if (sprindexArray.arraySize != assignedActor.Visuals.Length)
                {
                    int oldSize = sprindexArray.arraySize;
                    sprindexArray.arraySize = assignedActor.Visuals.Length;
                    if (oldSize < sprindexArray.arraySize)
                    {
                        int i = sprindexArray.arraySize - 1;
                        while (i >= oldSize)
                        {
                            sprindexArray.GetArrayElementAtIndex(i).stringValue = "";
                            i--;
                        }
                    }
                }

                for (int i = 0; i < sprindexArray.arraySize; i++)
                {
                    string stringAtI = sprindexArray.GetArrayElementAtIndex(i).stringValue;
                    string[] src = assignedActor.Visuals[i].visualIDs.ToArray();
                    Rect justAfterArray = new Rect(sprinRect.x, sprinRect.y + (EditorGUIUtility.singleLineHeight * (i + 1) + EditorGUIUtility.standardVerticalSpacing * (i + 1)), newR.width, EditorGUIUtility.singleLineHeight);
                    //string log = "Exising IDs: ";
                    //foreach(string s in src)
                    //{
                    //    log += s + ", ";
                    //}
                    //Debug.Log(log);
                    sprindexArray.GetArrayElementAtIndex(i).stringValue = 
                        EditorExtend.TextFieldAutoComplete(justAfterArray, stringAtI, src, 10);
                }
            }
            EditorGUI.EndChangeCheck();
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Drawer_Helper.PropertyHeight(property) + EditorGUIUtility.singleLineHeight * 12;
        }
    }
    [CustomPropertyDrawer(typeof(Actor_Expression))]
    public class Actor_Expression_Drawer : Screen_Event_Drawer
    {
        SerializedProperty actorProperty = null;
        SerializedProperty sprindexArray;
        Actor actorToAlter;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            m_SE = property;
            Rect dDownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            DropDownMenu(dDownRect, property);
            Rect newR = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height);
            EditorGUI.PropertyField(newR, property, new GUIContent("Change Actor Expression"), true);


            actorProperty = property.FindPropertyRelative("actorToAlter");
            sprindexArray = property.FindPropertyRelative("visualSpriteIndexes");
            actorToAlter = actorProperty.objectReferenceValue as Actor;

            EditorGUI.BeginChangeCheck();

            //if(actorToAlter != null) Debug.Log(actorToAlter.ActorName);
            Rect actRect = new Rect(newR.x, newR.y + EditorGUIUtility.singleLineHeight, newR.width, EditorGUIUtility.singleLineHeight * 2);
            Rect sprinRect = new Rect(newR.x, newR.y + (EditorGUIUtility.singleLineHeight * 2.5f), newR.width, EditorGUIUtility.singleLineHeight * 2);

            EditorGUI.PropertyField(actRect, actorProperty, new GUIContent("Actor to Alter"), true);
            EditorGUI.PropertyField(sprinRect, sprindexArray, new GUIContent("Visual Sprite Indexes"), true);

            
            if (actorToAlter != null)
            {
                if (sprindexArray.arraySize != actorToAlter.Visuals.Length)
                {
                    int oldSize = sprindexArray.arraySize;
                    sprindexArray.arraySize = actorToAlter.Visuals.Length;
                    if (oldSize < sprindexArray.arraySize)
                    {
                        int i = sprindexArray.arraySize - 1;
                        while (i >= oldSize)
                        {
                            sprindexArray.GetArrayElementAtIndex(i).stringValue = ""; //clear out newly generated values to not be dupes (which is bad!)
                            i--;
                        }
                    }
                }

                for (int i = 0; i < sprindexArray.arraySize; i++)
                {
                    string stringAtI = sprindexArray.GetArrayElementAtIndex(i).stringValue;
                    string[] src = actorToAlter.Visuals[i].visualIDs.ToArray();
                    Rect justAfterArray = new Rect(sprinRect.x, sprinRect.y + (EditorGUIUtility.singleLineHeight * (i + 1) + 
                        EditorGUIUtility.standardVerticalSpacing * (i + 1)), newR.width, EditorGUIUtility.singleLineHeight);
                    sprindexArray.GetArrayElementAtIndex(i).stringValue =
                        EditorExtend.TextFieldAutoComplete(justAfterArray, stringAtI, src, 10);
                }
            }

            EditorGUI.EndChangeCheck();

            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Drawer_Helper.PropertyHeight(property) + EditorGUIUtility.singleLineHeight * 12;
        }
    }
}
#endif