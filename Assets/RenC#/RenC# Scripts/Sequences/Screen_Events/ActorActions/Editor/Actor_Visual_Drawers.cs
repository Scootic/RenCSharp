#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using RenCSharp.Actors;
namespace RenCSharp.Sequences
{
    [CustomPropertyDrawer(typeof(Spawn_Actor))]
    public class Spawn_Actor_Drawer : Screen_Event_Drawer //tweaking sometimes, but not the other one?!?
    {
        private SerializedProperty spawnOffset = null;
        private SerializedProperty fadeInTime;
        private SerializedProperty sprindexArray;
        private SerializedProperty actorProperty;
        private Actor assignedActor;

        private ObjectField actorField;
        private FloatField fadeInTimeField;
        private Vector3Field spawnOffsetField;
        private AutoTextField[] autoTextFields;
        private static SerializedObject theSequence;
        private bool atfExists = false;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SetDropDownUITK(property);
            atfExists = false;

            actorProperty = property.FindPropertyRelative("actorToSpawn");
            theSequence = actorProperty.serializedObject;
            actorField = new ObjectField("Actor:");
            actorField.objectType = typeof(Actor);
            actorField.value = actorProperty.boxedValue as Object;
            assignedActor = actorField.value as Actor; //?
            container.Add(actorField);

            spawnOffset = property.FindPropertyRelative("spawnOffset");
            spawnOffsetField = new Vector3Field("Spawn Offset:");
            spawnOffsetField.tooltip = "Spawn Offset in local space.";
            spawnOffsetField.value = spawnOffset.vector3Value;
            container.Add(spawnOffsetField);
            spawnOffsetField.RegisterValueChangedCallback(evt =>
            {
                spawnOffset.vector3Value = evt.newValue;
                Debug.Log($"evtNewVal: {evt.newValue} | spawnOffsetValue: {spawnOffset.vector3Value}");
                theSequence.ApplyModifiedProperties();
            });
            
            fadeInTime = property.FindPropertyRelative("fadeInTime");
            fadeInTimeField = new FloatField("Fade in Time:");
            fadeInTimeField.tooltip = "Time in seconds it takes for the actor to fade in.";
            fadeInTimeField.value = fadeInTime.floatValue;
            container.Add(fadeInTimeField);
            fadeInTimeField.RegisterValueChangedCallback(evt =>
            {
                fadeInTime.floatValue = evt.newValue;
                theSequence.ApplyModifiedProperties();
            });
            

            sprindexArray = property.FindPropertyRelative("visualSpriteIndexes");
            //this last one works?!?
            actorField.RegisterValueChangedCallback(evt =>
            {
                actorProperty.boxedValue = evt.newValue;
                SetAutoTextFields(evt.newValue, property);
            });
            if (actorField.value != null)
            {
                SetAutoTextFields(actorField.value, property);
            }

            return container;
        }

        private void SetAutoTextFields(Object obj, SerializedProperty sp)
        {
            assignedActor = obj as Actor;
            actorProperty = sp.FindPropertyRelative("actorToSpawn");
            actorProperty.boxedValue = assignedActor;
            sprindexArray = sp.FindPropertyRelative("visualSpriteIndexes");
            if (atfExists)
            {
                foreach (AutoTextField atf in autoTextFields)
                {
                    container.Remove(atf);
                }
            }

            autoTextFields = new AutoTextField[assignedActor.Visuals.Length];
            atfExists = true;

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

            for (int i = 0; i < autoTextFields.Length; i++)
            {
                int disposeI = i;
                autoTextFields[disposeI] = new AutoTextField($"Visual Index {disposeI}:", assignedActor.Visuals[disposeI].visualIDs);
                string stValue = sprindexArray.GetArrayElementAtIndex(disposeI).stringValue;
                if (stValue != null)
                {
                    autoTextFields[disposeI].GetInputField.value = stValue;
                    autoTextFields[disposeI].OnKeyInput(stValue);
                    autoTextFields[disposeI].GetDropdownField.value = stValue;
                }
                autoTextFields[disposeI].RegisterValueChangedCallback(evt =>
                {
                    if (disposeI >= sprindexArray.arraySize) return;
                    sprindexArray.GetArrayElementAtIndex(disposeI).stringValue = evt.newValue;
                    theSequence.ApplyModifiedProperties();
                });
                container.Add(autoTextFields[disposeI]);
            }
            theSequence.ApplyModifiedProperties();
        }

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
        private SerializedProperty actorProperty = null;
        private SerializedProperty sprindexArray;
        private Actor actorToAlter;

        private ObjectField actorField;
        private AutoTextField[] autoTextFields;
        private bool atfExists = false;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SetDropDownUITK(property);
            atfExists = false;

            actorProperty = property.FindPropertyRelative("actorToAlter");
            actorField = new ObjectField("Actor:");
            actorField.objectType = typeof(Actor);
            actorField.value = actorProperty.boxedValue as Object;
            container.Add(actorField);

            sprindexArray = property.FindPropertyRelative("visualSpriteIndexes");
            actorField.RegisterValueChangedCallback(evt =>
            {
                actorProperty.boxedValue = evt.newValue;
                SetAutoTextFields(evt.newValue, property);
            });
            if (actorField.value != null)
            {
                SetAutoTextFields(actorField.value, property);
            }

            return container;
        }

        private void SetAutoTextFields(Object obj, SerializedProperty sp)
        {
            actorToAlter = obj as Actor;
            actorProperty = sp.FindPropertyRelative("actorToAlter");
            actorProperty.boxedValue = actorToAlter;
            sprindexArray = sp.FindPropertyRelative("visualSpriteIndexes");
            if (atfExists)
            {
                foreach (AutoTextField atf in autoTextFields)
                {
                    container.Remove(atf);
                }
            }

            autoTextFields = new AutoTextField[actorToAlter.Visuals.Length];
            atfExists = true;

            if (sprindexArray.arraySize != actorToAlter.Visuals.Length)
            {
                int oldSize = sprindexArray.arraySize;
                sprindexArray.arraySize = actorToAlter.Visuals.Length;
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

            for (int i = 0; i < autoTextFields.Length; i++)
            {
                int disposeI = i;
                autoTextFields[disposeI] = new AutoTextField($"Visual Index {disposeI}:", actorToAlter.Visuals[disposeI].visualIDs);
                string stValue = sprindexArray.GetArrayElementAtIndex(disposeI).stringValue;
                if (stValue != null)
                {
                    autoTextFields[disposeI].GetInputField.value = stValue;
                    autoTextFields[disposeI].OnKeyInput(stValue);
                    autoTextFields[disposeI].GetDropdownField.value = stValue;
                }
                autoTextFields[disposeI].RegisterValueChangedCallback(evt =>
                {
                    if (disposeI >= sprindexArray.arraySize) return;
                    sprindexArray.GetArrayElementAtIndex(disposeI).stringValue = evt.newValue;
                    sprindexArray.serializedObject.ApplyModifiedProperties();
                });
                container.Add(autoTextFields[disposeI]);
            }
            actorProperty.serializedObject.ApplyModifiedProperties();
        }

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