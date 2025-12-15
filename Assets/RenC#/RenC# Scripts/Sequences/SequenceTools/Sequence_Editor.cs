#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// I hate this so much. Try to give screen events their name, so you actually know what the hell they are.
    /// Also, gives a stupid ah dropdown to swap out event types within the sequence.
    /// </summary>
    [CustomPropertyDrawer(typeof(Screen_Event))]
    public class Screen_Event_Drawer : PropertyDrawer
    {
        private static Assembly childrenOfSE = Assembly.GetAssembly(typeof(Screen_Event));
        private static Type[] allSubs;
        [SerializeReference] protected SerializedProperty m_SE;

        //literally just default GUI but you can tell what the name of the screen event it is
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            m_SE = property;
            Rect dDownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            DropDown(dDownRect, property);
            Rect newPos = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height);
            EditorGUI.PropertyField(newPos, property, new GUIContent(property.type), true);
            EditorGUI.EndProperty();
        }

        protected void DropDown(Rect position, SerializedProperty property)
        {
            if (EditorGUI.DropdownButton(position, new GUIContent("Swap Event Type"), FocusType.Keyboard))
            {
                allSubs = childrenOfSE.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(Screen_Event))).ToArray();
                GenericMenu menu = new GenericMenu();

                foreach (Type stupid in allSubs)
                {
                    Screen_Event sumba = Activator.CreateInstance(stupid) as Screen_Event;
                    menu.AddItem(new GUIContent(sumba.ToString()),
                        !(property.managedReferenceValue == null) ? property.managedReferenceValue.ToString() == sumba.ToString() : false,
                        SetSEType, sumba);
                }

                menu.DropDown(position);
            }
        }

        public void SetSEType(object obj) //override the cur SE with the newly invented SE that's actually a type.
        {
            Debug.Log("Should be swappening the SEs");
            Screen_Event se = obj as Screen_Event;
            m_SE.managedReferenceValue = se;
            m_SE.serializedObject.ApplyModifiedProperties();
            Debug.Log(m_SE.type + "type1");
            Debug.Log(m_SE.managedReferenceValue.ToString() + "Type");
        }

        // >:(
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float returner = Drawer_Helper.PropertyHeight(property);
            returner += EditorGUIUtility.singleLineHeight * 2;
            return returner;
        }
    }


    /// <summary>
    /// Unique EditorWindow just to add specific screen events to a screen. Why? The Unity inspector was not built for polymorphism. FML.
    /// </summary>
    //public class Sequence_Editor : EditorWindow
    //{


    //private Vector2 scrollPos;
    //[Min(0), Tooltip("The screen that will receive the new screen action.")]private int screenIndex = 0;
    //Sequence manToEdit = null;

    // [MenuItem("Window/Sequence Editor")]
    // public static void ShowWindow()
    // {
    //     allSubs = childrenOfSE.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(Screen_Event))).ToArray();
    //     GetWindow(typeof(Sequence_Editor));
    // }

    // private void OnEnable()
    // {
    //      titleContent = new GUIContent("Sequence Editor");
    // }

    //  private void OnGUI()
    //   {
    //     GUILayout.Label("Sequence Data");
    //      manToEdit = (Sequence) EditorGUILayout.ObjectField("Sequence", manToEdit, typeof(Sequence), false);
    //      screenIndex = EditorGUILayout.IntField("Screen Index", screenIndex);
    //      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
    //freak the hell out
    //      if (allSubs == null) 
    //     GUILayout.Label("Screen Actions");
    //     foreach (Type stupid in allSubs)
    //     {
    //        Screen_Event sumba = Activator.CreateInstance(stupid) as Screen_Event;
    //        if (EditorGUILayout.LinkButton(sumba.ToString()))
    //         {
    //            if(screenIndex < manToEdit.Screens.Length)
    //             {
    //                 manToEdit.Screens[screenIndex].ScreenActions.Add(sumba);
    //               }
    //           else
    //            {
    //               Debug.LogWarning("Screen Index too large, you dingus!");
    //          }
    //       }
    //   }
    //       EditorGUILayout.EndScrollView();
    //  }

    ///old method below where buttons were spawned directly in Sequence inspectorGUI, rather than in their own window

    /*
    //reasonable stuff here
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Sequence selected = target as Sequence;
        Screen[] myTarget = selected.Screens;
        GUILayout.Label("Screen Index");
        screenIndex = GUILayout.TextField(screenIndex, 3); //set the index of the screen you want to add actions to
        GUILayout.Label("Possible Screen Actions");
        foreach (Type stupid in allSubs) //nested AF! HORRID!
        {
            Screen_Event sumba = Activator.CreateInstance(stupid) as Screen_Event; //get a class instance out of the type chicanery

            if (GUILayout.Button(sumba.ToString())) //create a button for each type that'll add that class to the screen actions[]
            {
                if (int.TryParse(screenIndex, out int result) && result >= 0)
                {
                    if (result < myTarget.Length)
                    {
                        myTarget[result].ScreenActions.Add(sumba);
                    }
                    else
                    {
                        Debug.LogWarning("Screen Index too big! Out of bounds!");
                    }
                }
                else
                {
                    Debug.LogWarning("Bad screen index assigned! (0 through 999 please)");
                }
            }
        }
    }*/
    //}

}
#endif