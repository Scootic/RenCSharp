#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

    [CustomEditor(typeof(Sequence)), CanEditMultipleObjects]
    public class Sequence_Editor : Editor
    {
        private Sequence _target;
        public override void OnInspectorGUI()
        {
            
            if(GUILayout.Button("Replace Deprecated Events"))
            {
                foreach (Object obj in targets)
                {
                    _target = (Sequence)obj;
                    int replaced = 0;
                    for (int i = 0; i < _target.Screens.Length; i++) //run through every single screen
                    {
                        Screen s = _target.Screens[i];
                        for (int j = 0; j < s.ScreenActions.Count; j++)
                        {
                            Screen_Event vent = s.ScreenActions[j];
                            if (vent.ToString() == "Deprecated/Play Music Track")
                            {
                                Debug.Log("Replacing BGM Play...");
                                Play_BGM bgmVent = vent as Play_BGM;
                                Play_BGMAsset newVent = new Play_BGMAsset();

                                newVent.SetFadeTime = bgmVent.FadeTime;
                                newVent.SetToSameTime = bgmVent.SetToSameTime;
                                s.ScreenActions[j] = newVent;
                                string johnson = AssetDatabase.GetAssetPath(bgmVent.Song);
                                string guid = AssetDatabase.AssetPathToGUID(johnson);
                                newVent.SetSongAsset = new AssetReference(guid);
                                replaced++;
                            }
                            else if(vent.ToString() == "Deprecated/Set Overlay Image")
                            {
                                Debug.Log("Replacing Set Overlay...");
                                Set_Overlay oldVent = vent as Set_Overlay;
                                Set_OverlayAsset newVent = new Set_OverlayAsset();

                                newVent.SetOverlayText = oldVent.GetOverlayText;
                                newVent.SetSecondsPerFrame = oldVent.GetSecondsPerFrame;
                                newVent.SetEndWithScreen = oldVent.GetEndWithScreen;
                                newVent.SetFadeTime = oldVent.GetFadeTime;

                                List<Sprite> sprungles = oldVent.GetImagesToSet;
                                List<AssetReferenceSprite> leRefs = new();
                                foreach(Sprite spr in sprungles) //get shiz set up!
                                {
                                    string johnson = AssetDatabase.GetAssetPath(spr);
                                    Debug.Log("Asset Path for sprite? " + johnson);
                                    string guid = AssetDatabase.AssetPathToGUID(johnson);
                                    AssetReferenceSprite stupid = new AssetReferenceSprite(guid);
                                    stupid.SubObjectName = spr.name; //?
                                    leRefs.Add(stupid);

                                }
                                newVent.SetImagesToSet = leRefs;
                                s.ScreenActions[j] = newVent;
                                replaced++;
                            }
                            else if(vent.ToString() == "Deprecated/Fade Transition")
                            {
                                Debug.Log("Replacing Fade Transition...");
                                Fade_Transition oldVent = vent as Fade_Transition;
                                Fade_TransitionAsset newVent = new Fade_TransitionAsset();

                                newVent.SetFadeTransition = oldVent.GetFadeTransition;
                                newVent.SetFadeDuration = oldVent.GetFadeDuration;
                                newVent.SetSecondsPerFrame = oldVent.GetSecondsPerFrame;

                                Sprite[] dingus = oldVent.GetNewBG;
                                List<AssetReferenceSprite> leRefs = new();
                                foreach (Sprite spr in dingus)
                                {
                                    string johnson = AssetDatabase.GetAssetPath(spr);
                                    string guid = AssetDatabase.AssetPathToGUID(johnson);
                                    AssetReferenceSprite stupid = new AssetReferenceSprite(guid);
                                    stupid.SubObjectName = spr.name;
                                    leRefs.Add(stupid);
                                }
                                newVent.SetNewBG = leRefs;
                                s.ScreenActions[j] = newVent;
                                replaced++;
                            }
                        }
                    }
                    EditorUtility.SetDirty(obj);
                    Debug.Log("Done replacing " + _target.name + "'s deprecated events! Replaced: " + replaced + " events.");
                }
            }
            base.OnInspectorGUI();
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