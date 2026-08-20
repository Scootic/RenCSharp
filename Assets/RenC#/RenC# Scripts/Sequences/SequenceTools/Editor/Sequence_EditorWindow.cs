#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RenCSharp.Sequences.Editor
{
    /// <summary>
    /// Unique EditorWindow for Sequences that exists because ScriptableObjects/Monobehaviors that have large arrays of strings
    /// run poorly when just using IMGUI. Every sequence has a potentially large array of strings with more stuff attached too,
    /// so editing it can get pretty slow, especially when conditional screens enter the mix. IMGUI Inspector still exists,
    /// purely as a way to verify that the EditorWindow didn't make any unwanted changes, or that it didn't apply desired changes.
    /// </summary>
    public class Sequence_EditorWindow : EditorWindow
    {
        [SerializeField] private static VisualTreeAsset _treeAsset = default;
        [SerializeField] private static Sequence _target;
        private static Sequence_EditorWindow SEW;
        private static Dictionary<int, Action> AllTheExtractScreens;
        private static Dictionary<int, Action> AllTheExtractPlayerChoices;
        public static Sequence SetTarget { set { _target = value; } }

        private ObjectField targetSequenceField;
        private Toggle autoSpeakerToggle;
        private PropertyField myAssetRefField;
        private ListView screenScrollView, playerchoiceScrollView;
        private Button addScreenButton, removeLastScreenButton, replaceDeprecatedEventsButton, reinitListButton;
        private Button addPcButton, removeLastPcButton, reinitPcButton;
        private Button saveSequenceButton;
        private IntegerField timeToAutoSaveField;
        private GradientField screenListCosmeticField;
        private float curT, timeToAutoSave = 120f;
        private Gradient screenListGradient;

        private readonly string _filePath = "Assets/RenC#/RenC# Scripts/Sequences/SequenceTools/UIToolkit for Sequences/Sequence_EditorWindow.uxml";

        [MenuItem("Window/Sequence Editor")]
        public static void OpenWindow()
        {
            SEW = GetWindow<Sequence_EditorWindow>();
            SEW.titleContent = new GUIContent("Sequence Editor");
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
            AllTheExtractScreens = new();
            AllTheExtractPlayerChoices = new();
        }

        private void NewSequenceSelected(ChangeEvent<UnityEngine.Object> changeEvent)
        {
            Debug.Log("Editor Window: Swapping out sequence!");
            ReserializeSequence();
            _target = changeEvent.newValue as Sequence;
            curT = 0;
            //change the stinking thing to display new stuff!

            SerializedObject so = new SerializedObject(_target);
            AllTheExtractScreens = new();
            AllTheExtractPlayerChoices = new();
            autoSpeakerToggle.value = _target.AutoFocusSpeaker;
            myAssetRefField.BindProperty(so.FindProperty("myself"));

            InitScreenListView();
            InitPlayerChoiceListView();
        }

        private void ReserializeSequence()
        {
            if (_target == null) { Debug.LogWarning("There is no sequence to reserialize (save)."); return; }
            _target.SetAFS = autoSpeakerToggle.value;
            foreach(KeyValuePair<int, Action> kvp in AllTheExtractPlayerChoices)
            {
                kvp.Value?.Invoke();
            }
            foreach(KeyValuePair<int, Action> kvp in AllTheExtractScreens)
            {
                kvp.Value?.Invoke();
            }
            //don't need to set the assetref, since it's a bound serialized property???
            EditorUtility.SetDirty(_target);
            AssetDatabase.SaveAssets();
        }

        private void InitScreenListView()
        {
            if (_target != null)
            {
                SerializedObject so = new SerializedObject(_target);
                SerializedProperty screensProp = so.FindProperty("screens");
                screenScrollView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
                screenScrollView.pickingMode = PickingMode.Ignore;
                screenScrollView.itemsSource = _target.Screens;
                screenScrollView.makeItem = () => new ScreenUITKField("Screen");
                screenScrollView.bindItem = (VisualElement e, int index) =>
                {
                    ScreenUITKField screenField = e as ScreenUITKField;
                    if (screenField != null)
                    {
                        if (index >= _target.Screens.Length) return;
                        int storedIndex = index;
                        float perc = (float)storedIndex / _target.Screens.Length; 
                        //Debug.Log("Screens Index: " + index);
                        SerializedProperty screenProp = screensProp.GetArrayElementAtIndex(storedIndex);
                        SerializedProperty screenActionsProp = screenProp.FindPropertyRelative("ScreenActions");
                        //Debug.Log($"Screen Action Property at: {index}" + screenActionsProp);
                        screenField.SetValue = _target.Screens[storedIndex];
                        screenField.SetScreenEventsProperty = screenActionsProp;
                        screenField.style.color = screenListGradient.Evaluate(perc);
                        screenField.style.backgroundColor = screenListGradient.Evaluate(perc);
                        screenField.SetCustomLabel(new Label($"Screen {storedIndex}"));
                        Action extract = delegate
                        {
                            _target.Screens[storedIndex].SetSpeaker = screenField.GetActor;
                            _target.Screens[storedIndex].SetDialog = screenField.GetDialog;
                        };
                        AllTheExtractScreens.Add(storedIndex, extract);
                    }
                };
                screenScrollView.unbindItem = (VisualElement e, int index) =>
                {
                    ScreenUITKField screenField = e as ScreenUITKField;
                    screenField.Unbind();
                    int storedIndex = index;
                    if (screenField != null)
                    {
                        AllTheExtractScreens.Remove(storedIndex);
                    }
                };
                screenScrollView.selectionType = SelectionType.None;
            }
        }

        private void InitPlayerChoiceListView() 
        { 
            if(_target != null)
            {
                SerializedObject so = new SerializedObject(_target);
                SerializedProperty playerchoicesProp = so.FindProperty("playerChoices");
                playerchoiceScrollView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

                playerchoiceScrollView.itemsSource = _target.PlayerChoices;
                playerchoiceScrollView.makeItem = () => new PlayerChoiceUITKField("Player Choice");
                playerchoiceScrollView.bindItem = (VisualElement e, int index) =>
                {
                    PlayerChoiceUITKField pcField = e as PlayerChoiceUITKField;
                    if (pcField != null)
                    {
                        int storedIndex = index;
                        pcField.SetPlayerChoice = _target.PlayerChoices[storedIndex];
                        pcField.SetConditionsProperty = playerchoicesProp.GetArrayElementAtIndex(storedIndex).FindPropertyRelative("conditions");
                        Action extractpc = delegate
                        {
                            _target.PlayerChoices[storedIndex].SetChoiceText = pcField.GetChoiceText;
                            _target.PlayerChoices[storedIndex].SetResultingSequence = pcField.GetResultingSequence;
                        };
                        AllTheExtractPlayerChoices.Add(storedIndex, extractpc);
                    }
                };
                playerchoiceScrollView.unbindItem = (VisualElement e, int index) =>
                {
                    PlayerChoiceUITKField pcField = e as PlayerChoiceUITKField;
                    if (pcField != null)
                    {
                        int storedIndex = index;
                        pcField.Q<PropertyField>().Unbind();
                        AllTheExtractPlayerChoices.Remove(storedIndex);
                    }
                };

                playerchoiceScrollView.selectionType = SelectionType.Single;
            }
        }

        public void CreateGUI()
        {
            _treeAsset = AssetDatabase.LoadAssetAtPath(_filePath, typeof(VisualTreeAsset)) as VisualTreeAsset;
            VisualElement root;
            try
            {
                root = _treeAsset.CloneTree();
            }
            catch (NullReferenceException)
            {
                Debug.LogError($"Couldn't find VisualTreeAsset at: {_filePath}. Either you moved or deleted it. Too bad!");
                return;
            }
            curT = 0;
            AllTheExtractScreens = new();
            AllTheExtractPlayerChoices = new();
            targetSequenceField = root.Q<ObjectField>("_target");
            targetSequenceField.value = _target;
            targetSequenceField.RegisterValueChangedCallback(NewSequenceSelected);

            screenListCosmeticField = root.Q<GradientField>("screenListCosmetic");
            if(screenListGradient != null) screenListCosmeticField.value = screenListGradient;
            screenListCosmeticField.RegisterValueChangedCallback((evt) =>
            {
                screenListGradient = evt.newValue;
                InitScreenListView();
            });
            screenListGradient = screenListCosmeticField.value;

            timeToAutoSaveField = root.Q<IntegerField>("autoSaveTime");
            timeToAutoSaveField.value = Mathf.RoundToInt(timeToAutoSave / 60f);
            timeToAutoSaveField.RegisterValueChangedCallback(evt =>
            {
                timeToAutoSave = evt.newValue * 60; //convert from minutes in field to seconds in code.
            });

            autoSpeakerToggle = root.Q<Toggle>("autoFocusSpeaker");
            if (_target != null) autoSpeakerToggle.value = _target.AutoFocusSpeaker;

            myAssetRefField = root.Q<PropertyField>("myself");
            if (_target != null) myAssetRefField.BindProperty(new SerializedObject(_target).FindProperty("myself"));

            screenScrollView = root.Q<ListView>("screenListView");

            reinitListButton = root.Q<Button>("reinitList");
            reinitListButton.tooltip = "Re-initializes the screen list. Use this if your list is bugging out visually. " +
            "You will probably need to do this after using the add screen button. (EVEN THOUGH THAT BUTTON ALSO REINITS THE LIST?!?) Hotkey: ctrl+r";
            reinitListButton.clicked += InitScreenListView;

            saveSequenceButton = root.Q<Button>("saveSequence");
            saveSequenceButton.tooltip = "Reserializes the target sequence, saving changes you've made in this editor window to the sequence object. "
            + "Changes made to the sequence SHOULD automatically be saved, but if you don't trust that: here you go. Hotkey: ctrl+s";
            saveSequenceButton.clicked += ReserializeSequence;

            addScreenButton = root.Q<Button>("addAScreen");
            addScreenButton.tooltip = "Adds a screen to the end of the list.";
            addScreenButton.clicked += delegate
            {
                if (_target == null) return;
                ReserializeSequence();
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
                        //type agnosticism?!?!?!?!?
                        IDeprecatedReplaceable<Screen_Event> replacer = vent as IDeprecatedReplaceable<Screen_Event>;
                        if (replacer != null)
                        {
                            s.ScreenActions[j] = replacer.Replacement();
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
                ReserializeSequence();
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
            reinitPcButton.tooltip = "Re-initializes the player choice list. Similar in fuction to reinit screen list button. Hotkey: ctrl+r (This hotkey will do both player choice list and screen list.)";
            reinitPcButton.clicked += InitPlayerChoiceListView;

            playerchoiceScrollView = root.Q<ListView>("playerChoiceListView");

            InitScreenListView();
            InitPlayerChoiceListView();

            rootVisualElement.Add(root);
        }

        private void OnGUI()
        { 
            Event cur = Event.current;
            if (cur.type != EventType.KeyDown || !cur.control) return; //only include events when ctrl is held down.

            switch (cur.keyCode)
            {
                case KeyCode.S:
                    Debug.Log($"Saving changes to sequence: {_target.name}.");
                    ReserializeSequence();
                    break;
                case KeyCode.R:
                    Debug.Log("Re-initializing screen and player choice lists.");
                    InitScreenListView();
                    InitPlayerChoiceListView();
                    break;
            }
        }

        private void OnInspectorUpdate()
        {
            if (_target == null) return;
            curT += 0.15f;
            if (curT >= timeToAutoSave && timeToAutoSave >= 0)
            {
                curT = 0;
                Debug.Log("Auto-saving sequence: " + _target.name);
                ReserializeSequence();
            }
        }
    }
}
#endif