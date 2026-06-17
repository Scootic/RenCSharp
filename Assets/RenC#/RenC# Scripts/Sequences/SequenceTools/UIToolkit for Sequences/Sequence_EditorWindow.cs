#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Device;
using UnityEngine.UIElements;

namespace RenCSharp.Sequences
{
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
        private ListView screenScrollView, playerchoiceScrollView;
        private Button addScreenButton, removeLastScreenButton, replaceDeprecatedEventsButton, reinitListButton;
        private Button addPcButton, removeLastPcButton, reinitPcButton;
        private Button saveSequenceButton;
        private Action ExtractTheScreens, ExtractPlayerChoices;

        private readonly string _filePath = "Assets/RenC#/RenC# Scripts/Sequences/SequenceTools/UIToolkit for Sequences/Sequence_EditorWindow.uxml";

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
            ReserializeSequence();
            _target = changeEvent.newValue as Sequence;
            //change the stinking thing to display new stuff!

            SerializedObject so = new SerializedObject(_target);
            ExtractTheScreens = null;
            ExtractPlayerChoices = null;
            autoSpeakerToggle.value = _target.AutoFocusSpeaker;
            myAssetRefField.BindProperty(so.FindProperty("myself"));

            InitScreenListView();
            InitPlayerChoiceListView();
        }

        private void ReserializeSequence()
        {
            if (_target == null) { Debug.LogWarning("There is no sequence to reserialize (save)."); return; }
            _target.SetAFS = autoSpeakerToggle.value;
            ExtractPlayerChoices?.Invoke();
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
                        Debug.Log("Screens Index: " + index);
                        SerializedProperty screenProp = screensProp.GetArrayElementAtIndex(index);
                        SerializedProperty screenActionsProp = screenProp.FindPropertyRelative("ScreenActions");
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

        private void InitPlayerChoiceListView() 
        { 
            if(_target != null)
            {
                SerializedObject so = new SerializedObject(_target);
                SerializedProperty playerchoicesProp = so.FindProperty("playerChoices");
                Debug.Log("Playerchoices prop: " + playerchoicesProp);
                playerchoiceScrollView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

                playerchoiceScrollView.itemsSource = _target.PlayerChoices;
                playerchoiceScrollView.makeItem = () => new PlayerChoiceUITKField("Player Choice");
                playerchoiceScrollView.bindItem = (VisualElement e, int index) =>
                {
                    PlayerChoiceUITKField pcField = e as PlayerChoiceUITKField;
                    if (pcField != null)
                    {
                        Debug.Log("Player choice Index: " + index);
                        pcField.SetPlayerChoice = _target.PlayerChoices[index];
                        pcField.SetConditionsProperty = playerchoicesProp.GetArrayElementAtIndex(index).FindPropertyRelative("conditions");
                        ExtractPlayerChoices += delegate
                        {
                            _target.PlayerChoices[index].SetChoiceText = pcField.GetChoiceText;
                            _target.PlayerChoices[index].SetResultingSequence = pcField.GetResultingSequence;
                        };
                    }
                };

                playerchoiceScrollView.selectionType = SelectionType.Single;
            }
        }

        public void CreateGUI()
        {
            _treeAsset = AssetDatabase.LoadAssetAtPath(_filePath, typeof(VisualTreeAsset)) as VisualTreeAsset;
            VisualElement root;
            //Debug.Log("Found the tree asset!");
            try
            {
                root = _treeAsset.CloneTree();
            }
            catch (NullReferenceException)
            {
                Debug.LogError($"Couldn't find VisualTreeAsset at: {_filePath}. Either you moved or deleted it. Too bad!");
                return;
            }
            ExtractTheScreens = null;
            ExtractPlayerChoices = null;
            targetSequenceField = root.Q<ObjectField>("_target");
            targetSequenceField.value = _target;
            targetSequenceField.RegisterValueChangedCallback(NewSequenceSelected);

            autoSpeakerToggle = root.Q<Toggle>("autoFocusSpeaker");
            if (_target != null) autoSpeakerToggle.value = _target.AutoFocusSpeaker;

            myAssetRefField = root.Q<PropertyField>("myself");
            if (_target != null) myAssetRefField.BindProperty(new SerializedObject(_target).FindProperty("myself"));

            screenScrollView = root.Q<ListView>("screenListView");

            reinitListButton = root.Q<Button>("reinitList");
            reinitListButton.tooltip = "Re-initializes the screen list. Use this if your list is bugging out visually. " +
            "You will probably need to do this after using the add screen button. (EVEN THOUGH THAT BUTTON ALSO REINITS THE LIST?!?)";
            reinitListButton.clicked += InitScreenListView;

            saveSequenceButton = root.Q<Button>("saveSequence");
            saveSequenceButton.tooltip = "Reserializes the target sequence, saving changes you've made in this editor window to the sequence object. "
            + "Changes made to the sequence SHOULD automatically be saved, but if you don't trust that: here you go.";
            saveSequenceButton.clicked += ReserializeSequence;

            addScreenButton = root.Q<Button>("addAScreen");
            addScreenButton.tooltip = "Adds a screen to the end of the list.";
            addScreenButton.clicked += delegate
            {
                if (_target == null) return;
                Screen[] temp = new Screen[_target.Screens.Length + 1];
                for (int i = 0; i < _target.Screens.Length; i++)
                {
                    temp[i] = _target.Screens[i];
                }
                temp[_target.Screens.Length] = new Screen();
                _target.SetScreens = temp;
                InitScreenListView();
            };

            removeLastScreenButton = root.Q<Button>("removeLastScreen");
            removeLastScreenButton.tooltip = "Removes the screen at the end of the list, shortening it.";
            removeLastScreenButton.clicked += delegate
            {
                if (_target == null || _target.Screens.Length <= 0) return;
                Screen[] temp = new Screen[_target.Screens.Length - 1];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = _target.Screens[i];
                }
                _target.SetScreens = temp;
                InitScreenListView();
            };

            replaceDeprecatedEventsButton = root.Q<Button>("replaceDeprecatedEvents");
            replaceDeprecatedEventsButton.tooltip = "Runs through the entire sequence, replacing deprecated events with their new versions.";
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

            addPcButton = root.Q<Button>("addApc");
            addPcButton.tooltip = "Adds a player choice to the end of the list.";
            addPcButton.clicked += delegate
            {
                if (_target == null) return;
                Player_Choice[] temp = new Player_Choice[_target.PlayerChoices.Length + 1];
                for(int i = 0; i < _target.PlayerChoices.Length; i++)
                {
                    temp[i] = _target.PlayerChoices[i];
                }
                temp[_target.PlayerChoices.Length] = new Player_Choice();
                _target.SetPlayerChoices = temp;
                InitPlayerChoiceListView();
            };

            removeLastPcButton = root.Q<Button>("removeLastpc");
            removeLastPcButton.tooltip = "Removes the player choice at the end of the list, shortening it.";
            removeLastPcButton.clicked += delegate
            {
                if (_target == null || _target.PlayerChoices.Length <= 0) return;
                Player_Choice[] temp = new Player_Choice[_target.PlayerChoices.Length - 1];
                for(int i = 0; i < temp.Length; i++)
                {
                    temp[i] = _target.PlayerChoices[i];
                }
                _target.SetPlayerChoices = temp;
                InitPlayerChoiceListView();
            };

            reinitPcButton = root.Q<Button>("reinitpcList");
            reinitPcButton.tooltip = "Re-initializes the player choice list. Similar in fuction to reinit screen list button.";
            reinitPcButton.clicked += InitPlayerChoiceListView;

            playerchoiceScrollView = root.Q<ListView>("playerChoiceListView");

            InitScreenListView();
            InitPlayerChoiceListView();

            rootVisualElement.Add(root);
        }
    }
}
#endif