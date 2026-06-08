#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// I hate this so much. Try to give screen events their name, so you actually know what the hell they are.
    /// Also, gives a stupid ah dropdown to swap out event types within the sequence.
    /// </summary>
    [CustomPropertyDrawer(typeof(Screen_Event))]
    public class Screen_Event_Drawer : PolymorphicPropertyDrawer<Screen_Event>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, true);
            if (EditorGUI.EndChangeCheck() && !EditorGUIUtility.editingTextField)
            {
                
                Debug.Log($"SErializing the Screener! {DropDownMenuName()}");
                property.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.EndProperty();
        }

        protected override string DropDownMenuName()
        {
            return "Select Screen Event Type";
        }
    }

    [CustomEditor(typeof(Sequence)), CanEditMultipleObjects]
    public class Sequence_Editor : Editor
    {
        private Sequence _target;
        private SerializedProperty _autoFocusSpeaker;
        private SerializedProperty _screens;
        private SerializedProperty _playerChoices;
        private SerializedProperty _myself;

        private void OnEnable()
        {
            _autoFocusSpeaker = serializedObject.FindProperty("autoFocusSpeaker");
            _screens = serializedObject.FindProperty("screens");
            _playerChoices = serializedObject.FindProperty("playerChoices");
            _myself = serializedObject.FindProperty("myself");
        }

        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Calculate Sequence Length")) 
            {
                int totalScreens = 0;
                int wordcount = 0;
                long totalChars = 0;

                foreach(Object obj in targets)
                {
                    _target = (Sequence)obj;
                    totalScreens += _target.Screens.Length;
                    foreach(Screen s in _target.Screens)
                    {
                        int i = 0;
                        char[] chars = s.Dialog.ToCharArray();
                        while (i < chars.Length)
                        {
                            while (i < chars.Length && !char.IsWhiteSpace(chars[i])) 
                            {
                                i++;
                                totalChars++;
                            }

                            wordcount++;

                            while (i < chars.Length && char.IsWhiteSpace(chars[i]))
                            {
                                i++;
                                totalChars++;
                            }
                        }
                    }
                }

                double totalSeconds = ((double)totalChars / 60d) + (totalScreens * 0.5f); //total characters divided by 60FPS (a char a frame) +
                                                                                          //linger time of 0.5seconds per screen.
                double totalMinutes = totalSeconds / 60d;
                double totalHours = totalMinutes / 60d;

                totalHours = Mathf.Floor((float)totalHours);
                totalMinutes -= totalHours * 60d;
                totalMinutes = Mathf.Floor((float) totalMinutes);
                totalSeconds -= totalMinutes * 60d;
                totalSeconds = Mathf.Floor((float)totalSeconds);

                Debug.Log($"Total Found Screens: {totalScreens}." +
                    $"\nTotal Found Characters: {totalChars}. Total Word Count: {wordcount}" +
                    $"\nAssuming 60FPS and left on auto at fastest possible speed (linger time of 0.5s), the total time *should* be:" +
                    $"\n{totalHours}:{totalMinutes}:{totalSeconds} (H:M:S). (Ignores fade transitions and other pauses to sequence.)");
            }
            
            if(GUILayout.Button("Replace Deprecated Events")) //pretty please update to include things you want to replace en masse
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

            EditorGUILayout.PropertyField(_autoFocusSpeaker);
            EditorGUILayout.PropertyField(_myself);
            EditorGUILayout.PropertyField(_screens);
            EditorGUILayout.PropertyField(_playerChoices);
        }

        private void OnDisable()
        {
            Debug.Log("Serualized The sequence on disabb0");
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }

    public class Sequence_EditorWindow : EditorWindow
    {
        private static Sequence manToEdit = null;

        private SerializedProperty _autoFocusSpeaker = null;
        private SerializedProperty _myself = null;
        private SerializedProperty _screens = null;
        private SerializedProperty _playerChoices = null;
        private SerializedObject _so = null;
        private SerializedProperty[] _displayedScreens;

        private bool autoFocus;
        private string stupid = "";

        private int screensScrollLength;
        private int screensToShowAtOnce = 5;
        private float screenScrollValue;
        private float screenScrollPerc;
        private float playerScrollValue;

        [MenuItem("Window/Sequence Editor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(Sequence_EditorWindow));
        }

        public static void ShowWindow(Sequence givenSequence)
        {
            GetWindow(typeof(Sequence_EditorWindow));
            manToEdit = givenSequence;
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Sequence Editor");
        }

        private void OnDisable()
        {
            if (_so == null) return;
            _so.ApplyModifiedProperties();
            _so.Update();
        }

        private void OnGUI()
        {
            GUILayout.Label("Sequence Data");
            EditorGUI.BeginChangeCheck();
            manToEdit = (Sequence)EditorGUILayout.ObjectField("Sequence", manToEdit, typeof(Sequence), false);
            if (manToEdit == null) return;
            if(_autoFocusSpeaker == null || EditorGUI.EndChangeCheck()) //should fire whenever the sequence field changes
            {
                _so = new SerializedObject(manToEdit);
                _autoFocusSpeaker = _so.FindProperty("autoFocusSpeaker");
                _myself = _so.FindProperty("myself");
                _screens = _so.FindProperty("screens").Copy();
                _playerChoices = _so.FindProperty("playerChoices").Copy();
                screensScrollLength = _screens.arraySize * 300;
                autoFocus = manToEdit.AutoFocusSpeaker;
                _displayedScreens = new SerializedProperty[screensToShowAtOnce];
            }
            autoFocus = EditorGUILayout.Toggle("Auto Focus Speaker", autoFocus);
            stupid = EditorGUILayout.TextField(stupid);
            EditorGUI.BeginChangeCheck();
            screensToShowAtOnce = EditorGUILayout.IntField("Screens to Show At Once", screensToShowAtOnce);
            if (EditorGUI.EndChangeCheck())
            {
                _displayedScreens = new SerializedProperty[screensToShowAtOnce];
            }
            EditorGUILayout.PropertyField(_myself);
            screenScrollValue = EditorGUILayout.BeginScrollView(new Vector2(0f, screenScrollValue)).y;
            
            if (_screens != null)
            {
                screenScrollPerc = screenScrollValue / (float)screensScrollLength;
                Debug.Log($"Screen scroll value: {screenScrollValue}. Screen scroll length: {screensScrollLength}. Perc: {screenScrollPerc * 100f}%.");
                //Instead of displaying a full array all at once, only show the screens we want.
                int rootIndex = Mathf.FloorToInt(screenScrollPerc * (_screens.arraySize - 1));
                for (int i = 0; i < screensToShowAtOnce; i++)
                {
                    int desIndex = rootIndex + i;
                    if (desIndex >= _screens.arraySize) break;
                    _displayedScreens[i] = _screens.GetArrayElementAtIndex(desIndex);
                    EditorGUILayout.PropertyField(_displayedScreens[i], true);
                }
                //EditorGUILayout.PropertyField(_screens)
                ///use ^^ for bs test mergenceries
            }
            EditorGUILayout.EndScrollView();
            if(EditorGUILayout.LinkButton("Add A Screen"))
            {
                _screens.InsertArrayElementAtIndex(_screens.arraySize);
            }
            playerScrollValue = EditorGUILayout.BeginScrollView(new Vector2(0f, playerScrollValue)).y;
            EditorGUILayout.PropertyField(_playerChoices, true);
            EditorGUILayout.EndScrollView();
        }
    }

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
    

}
#endif