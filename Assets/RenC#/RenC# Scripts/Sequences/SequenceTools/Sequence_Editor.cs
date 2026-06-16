#if UNITY_EDITOR
using RenCSharp.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEditor.UIElements;
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
            if (_target == null) _target = target as Sequence;

            if (GUILayout.Button("Calculate Sequence Length")) 
            {
                int totalScreens = 0;
                int wordcount = 0;
                long totalChars = 0;

                foreach(UnityEngine.Object obj in targets)
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
                foreach (UnityEngine.Object obj in targets)
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
                            else if (vent.ToString() == "Deprecated/Set Overlay Image")
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
                                foreach (Sprite spr in sprungles) //get shiz set up!
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
                            else if (vent.ToString() == "Deprecated/Fade Transition")
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

            if(GUILayout.Button("Open in Sequence Editor"))
            {
                Sequence_EditorWindow.OpenWindow(_target);
                Sequence_EditorWindow.SetTarget = _target;
            }

            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
        }
    }

    public class ScreenConverter : UxmlAttributeConverter<Screen> //not sure if i even need this?
    {
        static string ValueToString(object obj) => Convert.ToString(obj, CultureInfo.InvariantCulture);

        public override Screen FromString(string value)
        {
            Screen s = new Screen();
            object[] objects = value.Split('|');
            s.SetSpeaker = (Actor)Convert.ChangeType(objects[0], typeof(Actor), CultureInfo.InvariantCulture);
            s.SetDialog = objects[1] as string;
            s.ScreenActions = (List<Screen_Event>)Convert.ChangeType(objects[2], typeof(List<Screen_Event>), CultureInfo.InvariantCulture);
            return s;
       }

        public override string ToString(Screen s)
        {
            string format = "";
            format += ValueToString(s.Speaker + "|");
            format += s.Dialog + "|";
            format += ValueToString(s.ScreenActions);
            return format;
        }
    }

    public class ScreenUITKField : BaseField<Screen>
    {
        private readonly VisualElement ContentElement;
        private readonly ObjectField ActorField;
        private readonly TextField DialogField;
        private readonly PropertyField ScreenEventsField; //really sad. Maybe a converter just to turn this boy to uxml?
        private readonly VisualElement GapSpace;

        public ScreenUITKField(string labelText, Screen s) : base(labelText, new VisualElement())
        {
            ContentElement = this.Q<VisualElement>(className: inputUssClassName);
            ContentElement.style.flexDirection = FlexDirection.Column;
            AddToClassList(alignedFieldUssClassName);
            labelElement.style.marginBottom = 1;

            ActorField = new ObjectField();
            ActorField.label = "Speaker:";
            ActorField.objectType = typeof(Actor);
            ActorField.value = s.Speaker;
            ContentElement.Add(ActorField);

            DialogField = new TextField();
            DialogField.label = "Dialog:";
            DialogField.SetVerticalScrollerVisibility(ScrollerVisibility.Auto); //???
            DialogField.style.whiteSpace = WhiteSpace.Normal;
            DialogField.multiline = true;
            ContentElement.Add(DialogField);

            ScreenEventsField = new PropertyField();
            ScreenEventsField.label = "Screen Actions:";
            ContentElement.Add(ScreenEventsField);

            GapSpace = new VisualElement();
            GapSpace.style.height = 20;
            ContentElement.Add(GapSpace);
        }

        public ScreenUITKField(string labelText) : base(labelText, new VisualElement())
        {
            ContentElement = this.Q<VisualElement>(className: inputUssClassName);
            ContentElement.style.flexDirection = FlexDirection.Column;
            AddToClassList(alignedFieldUssClassName);
            labelElement.style.marginBottom = 1;

            ActorField = new ObjectField();
            ActorField.objectType = typeof(Actor);
            ActorField.label = "Speaker:";
            ActorField.value = null;
            ContentElement.Add(ActorField);

            DialogField = new TextField();
            DialogField.SetVerticalScrollerVisibility(ScrollerVisibility.Auto);
            DialogField.style.whiteSpace = WhiteSpace.Normal;
            DialogField.label = "Dialog:";
            DialogField.value = "";
            DialogField.multiline = true;
            ContentElement.Add(DialogField);

            ScreenEventsField = new PropertyField();
            ScreenEventsField.label = "Screen Actions:";
            ContentElement.Add(ScreenEventsField);

            GapSpace = new VisualElement();
            GapSpace.style.height = 20;
            ContentElement.Add(GapSpace);
        }

        public Screen SetValue
        {
            set
            {
                ActorField.value = value.Speaker;
                DialogField.value = value.Dialog;
            }
        }

        public Actor GetActor
        {
            get
            {
                return ActorField.value as Actor;
            }
        }

        public string GetDialog
        {
            get
            {
                return DialogField.value;
            }
        }

        public SerializedProperty SetScreenEventsProperty
        {
            set 
            {
                if (value != null)
                {
                    ScreenEventsField.BindProperty(value);
                }
                else
                {
                    Debug.LogError("SetScreenEventsProperty was passed a null property!");
                }
            }
        }

        public void SetCustomLabel(VisualElement customLabel)
        {
            labelElement.Clear();
            labelElement.Add(customLabel);
        }

        public void SetCustomContent(VisualElement customContent)
        {
            ContentElement.Clear();
            ContentElement.Add(customContent);
        }
    }


    /// <summary>
    /// Unique EditorWindow just to add specific screen events to a screen. Why? The Unity inspector was not built for polymorphism. FML.
    /// </summary>
    public class Sequence_EditorWindow : EditorWindow
    {
        [SerializeField] private static VisualTreeAsset _treeAsset = default;
        [SerializeField] private static Sequence _target;
        private static EditorWindow me;
        public static Sequence SetTarget { set { _target = value; } }

        private ObjectField targetSequenceField;
        private Toggle autoSpeakerToggle;
        private PropertyField myAssetRefField;
        private ListView screenScrollView;
        private Button addScreenButton, removeLastScreenButton, replaceDeprecatedEventsButton;
        private Action ExtractTheScreens;

        private readonly string _filePath = "Assets/RenC#/RenC# Scripts/Sequences/SequenceTools/Sequence_EditorWindow.uxml";

        [MenuItem("Window/Sequence Editor")]
        public static void OpenWindow()
        {
            me = GetWindow<Sequence_EditorWindow>();
            me.titleContent = new GUIContent("Sequence Editor");
        }

        public static void OpenWindow(Sequence newTarget)
        {
            _target = newTarget;
            OpenWindow();
        }

        private void OnDestroy()
        {
            //reserialize the stinkin' sequence
            ReserializeSequence();
            _target = null;
            targetSequenceField = null;
            autoSpeakerToggle = null;
            myAssetRefField = null;
            screenScrollView = null;
        }

        private void NewSequenceSelected(ChangeEvent<UnityEngine.Object> changeEvent)
        {
            Debug.Log("Editor Window: Swapping out sequence!");
            _target = changeEvent.newValue as Sequence;
            //change the stinking thing to display new stuff!

            SerializedObject so = new SerializedObject(_target);

            autoSpeakerToggle.value = _target.AutoFocusSpeaker;
            myAssetRefField.BindProperty(so.FindProperty("myself"));

            InitScreenListView();
        }

        private void ReserializeSequence()
        {
            if (_target == null) return;
            _target.SetAFS = autoSpeakerToggle.value;
            ExtractTheScreens?.Invoke();
            //don't need to set the assetref, since it's a bound serialized property???
            EditorUtility.SetDirty(_target);
        }

        private void InitScreenListView()
        {
            if (_target != null)
            {
                SerializedObject so = new SerializedObject(_target);
                SerializedProperty screensProp = so.FindProperty("screens");
                screenScrollView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
                screenScrollView.itemsSource = _target.Screens;
                screenScrollView.makeItem = () => new ScreenUITKField("Screen");
                screenScrollView.bindItem = (VisualElement e, int index) =>
                {
                    ScreenUITKField screenField = e as ScreenUITKField;
                    if (screenField != null)
                    {
                        if (index >= _target.Screens.Length) return;
                        //Debug.Log("Screens Property: " + screensProp);
                        SerializedProperty screenActionsProp = screensProp.GetArrayElementAtIndex(index).FindPropertyRelative("ScreenActions");
                        //Debug.Log($"Screen Action Property at: {index}" + screenActionsProp);
                        screenField.SetValue = _target.Screens[index];
                        screenField.SetScreenEventsProperty = screenActionsProp;
                        screenField.SetCustomLabel(new Label($"Screen {index}"));
                        ExtractTheScreens += delegate
                        {
                            _target.Screens[index].SetSpeaker = screenField.GetActor;
                            _target.Screens[index].SetDialog = screenField.GetDialog;
                        };
                    }
                };
                screenScrollView.selectionType = SelectionType.Single;
            }
        }

        public void CreateGUI()
        {
            try
            {
                _treeAsset = AssetDatabase.LoadAssetAtPath(_filePath, typeof(VisualTreeAsset)) as VisualTreeAsset;
            }
            catch (NullReferenceException)
            {
                Debug.LogError($"Couldn't find a .uxml file at: {_filePath}. Either you moved or deleted it. Sucks to be you!");
                return;
            }
            //Debug.Log("Found the tree asset!");
            VisualElement root = _treeAsset.CloneTree();
            ExtractTheScreens = null;
            targetSequenceField = root.Q<ObjectField>("_target");
            targetSequenceField.value = _target;
            targetSequenceField.RegisterValueChangedCallback(NewSequenceSelected);

            autoSpeakerToggle = root.Q<Toggle>("autoFocusSpeaker");
            if(_target != null)autoSpeakerToggle.value = _target.AutoFocusSpeaker;

            myAssetRefField = root.Q<PropertyField>("myself");

            screenScrollView = root.Q<ListView>("screenListView"); //WHAT IN THE actual FUcK

            addScreenButton = root.Q<Button>("addAScreen");
            addScreenButton.clicked += delegate
            {
                if (_target == null) return;
                Screen[] temp = new Screen[_target.Screens.Length + 1];
                for(int i = 0; i < _target.Screens.Length; i++)
                {
                    temp[i] = _target.Screens[i];
                }
                temp[_target.Screens.Length] = new Screen();
                _target.SetScreens = temp;
                InitScreenListView();
            };

            removeLastScreenButton = root.Q<Button>("removeLastScreen");
            removeLastScreenButton.clicked += delegate
            {
                if (_target == null) return;
                Screen[] temp = new Screen[_target.Screens.Length - 1];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = _target.Screens[i];
                }
                _target.SetScreens = temp;
                InitScreenListView();
            };

            replaceDeprecatedEventsButton = root.Q<Button>("replaceDeprecatedEvents");
            replaceDeprecatedEventsButton.clicked += delegate
            {
                if (_target == null) return;
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
                        else if (vent.ToString() == "Deprecated/Set Overlay Image")
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
                            foreach (Sprite spr in sprungles) //get shiz set up!
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
                        else if (vent.ToString() == "Deprecated/Fade Transition")
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
                    Debug.Log("Replaced " + replaced + " deprecated screen events.");
                    InitScreenListView();
                }
            };

            InitScreenListView();
            rootVisualElement.Add(root);
        }
    }

    ///old garbage window that tries to segment the Screen array to save on redraws. Doesn't friggin' work. I'm blaming
    ///serialization???
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