#if UNITY_EDITOR
using UnityEditor;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// I hate this so much. Try to give screen events their name, so you actually know what the hell they are.
    /// Also, gives a stupid ah dropdown to swap out event types within the sequence.
    /// </summary>
    [CustomPropertyDrawer(typeof(Screen_Event))]
    public class Screen_Event_Drawer : PolymorphicPropertyDrawer<Screen_Event>
    {
        protected override string DropDownMenuName()
        {
            return "Select Screen Event Type";
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