using RenCSharp.Actors;
using RenCSharp.Sequences;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using EXPERIMENTAL;
namespace RenCSharp
{
    /// <summary>
    /// The guy who handles the game logic. Runs through the screens of a sequence, and does all the stuff. For some god awful reason, does not
    /// like sequences that only have 1 screen in them. If you really desparetely need a 1 screen sequence, just include a completely empty screen
    /// as the second screen.
    /// </summary>
    public sealed class Script_Manager : MonoBehaviour
    {
        [SerializeField] private Sequence currentSequence; //set before runtime for first sequence of unity scene
        private int curScreenIndex = 0;

        [Header("Dialog Fields")]
        [SerializeField] private TextMeshProUGUI speakerNameField;
        [SerializeField] private TextMeshProUGUI dialogField;
        [SerializeField] private Image speakerNameBox;
        [SerializeField] private Image dialogBox;
        [SerializeField, Tooltip("Decides the color of the textboxes if there's no actor in a screen.")] private Color defaultTextBoxColor = Color.white;

        [Header("Buttons")]
        [SerializeField] private Button playerchoicePrefab;
        [SerializeField] private Button progressDialogButton;
        [SerializeField] private Image toggleImage;
        [SerializeField] private Color togglePressedColor;
        [SerializeField] private Transform playerchoiceHolder;
        
        [Header("Actors")]
        [SerializeField] private Transform actorHolder;
        [SerializeField] private AnimationCurve actorScalingKurve;
        private Actor curActor;
        [HideInInspector] public List<Actor> activeActors = new();

        [Header("Overlay")]
        [SerializeField] private GameObject overlayPrefab; //should have a TMPro child
        [SerializeField] private Transform overlayHolder;
        [SerializeField] private GameObject menuBase;

        [Header("Settings")]
        [SerializeField, Tooltip("In seconds, 0 for character every frame."), Min(0)] private float textSpeed = 0;
        [SerializeField, Tooltip("In seconds."), Min(0)] private float autoFocusScaleDuration = 0.25f;
        [SerializeField] private string playerName = "Guy"; //probably should be handled by an save data
        [SerializeField, Tooltip("This will be string that is replaced by inputted player name.")] private string playerTag = "{MC}";
        [SerializeField] private bool auto = false;
        [SerializeField, Tooltip("How long the SM will linger on a screen while on auto.")] private float lingerTime = 0.5f;
        [SerializeField, Tooltip("How many text boxes are remembered by history. Don't be zero.")] private byte historyLength = 10;

        [Header("Databases")]
        [SerializeField] private Sprite_Database overlayDatabase;
        [SerializeField] private Sprite_Database backgroundDatabase;
        [SerializeField] private Audio_Database audioDatabase;

        private bool jumpToEndDialog = false, paused = false, saving = false, loaded = false;
        private float curSpeed;
        private History curHist;

        public static Script_Manager SM;
        public static Action ProgressScreenEvent, EndOfAllSequencesEvent;
        public static Action<bool> SequencePausedEvent;
        public Transform ActorHolder => actorHolder;
        public History CurrentHistory => curHist;
        //certified singleton moment
        private void Awake()
        {
            if (SM == null)
            {
                SM = this;
            } else if (SM != this)
            {
                Destroy(this);
            }

            Object_Factory.SpawnObject(overlayPrefab, "Overlay", overlayHolder);
            Object_Factory.SpawnObject(overlayPrefab, "Background", GameObject.Find("BGcanv").transform);//horrid

            Flag_Manager.ReceiveFlagToken(SaveLoad.LoadPersistentFlags(), true); //safety thing

            curHist = new History(historyLength);
            textSpeed = PlayerPrefs.GetFloat("TextSpeed");
            lingerTime = PlayerPrefs.GetFloat("LingerTime");
            curSpeed = textSpeed;

            Event_Bus.AddFloatEvent("TextSpeed", TextSpeed);
            Event_Bus.AddFloatEvent("LingerTime", TextAutoHang);
            Event_Bus.AddStringEvent("PlayerName", SetPlayerName);

            EndOfAllSequencesEvent += Application.Quit; //TEMPORARY THING
            SequencePausedEvent += ToggleDialogUI;
        }
        private void Start()
        {
            if (!loaded) { StartSequence(); }
        }

        private void OnDisable()
        {
            Object_Factory.ScrubDictionary(); //the dictionary is static, so we don't want to keep storing garbage forever.
            FlagToken ft = new FlagToken(Flag_Manager.GetPersistentDataFlags);
            SaveLoad.SavePersistentFlags(ft);
            SM = null;
        }
        #region SequenceHandling
        public void StartSequence()
        {
            Debug.Log("Started Sequence: " + currentSequence.name);
            curScreenIndex = 0;
            StartCoroutine(RunThroughScreen(currentSequence.Screens[0]));
        }

        public void PauseSequence()
        {
            Debug.Log("SM PAUSED");
            paused = true;
            SequencePausedEvent?.Invoke(paused); //for cool mfs to do game stuff whenever the sequence is paused. minigame mayhaps?
        }

        public void UnpauseSequence()
        {
            Debug.Log("SM UNPAUSED");
            paused = false;
            SequencePausedEvent?.Invoke(paused);
        }
        private void LoadASequence(Sequence s)
        {
            //remove all player choice buttons after picking an option
            for (int i = playerchoiceHolder.childCount - 1; i >= 0; i--)
            {
                Destroy(playerchoiceHolder.GetChild(i).gameObject);
            }
            currentSequence = s;
            StartSequence();
        }
        #endregion
        #region ScreenHandling
        public void ProgressToNextScreen() //for an UI button to use, hopefully
        {
            if (paused) return; //the ui button's interactivity should be able to handle this automatically
            if (!jumpToEndDialog) jumpToEndDialog = true;
            else
            {
                Debug.Log("Moving to next screen!");
                ProgressScreenEvent?.Invoke();
                ProgressScreenEvent = null; //wipe all delegates from the action before continuing
                curScreenIndex++;
                Debug.Log("current Scrindex: " + curScreenIndex + ", Final Screen? " + (curScreenIndex >= currentSequence.Screens.Length - 1));
                if(curScreenIndex < currentSequence.Screens.Length) StartCoroutine(RunThroughScreen(currentSequence.Screens[curScreenIndex]));
                else if(curScreenIndex > currentSequence.Screens.Length - 1)//final screen of the sequence
                {
                    if (currentSequence.PlayerChoices.Length == 0)//if there are no valid next sequences, sum shit gone wrong
                    { 
                        Debug.Log("No next sequence, game over?"); 
                        EndOfAllSequencesEvent?.Invoke();
                        return; 
                    }

                    Player_Choice firstPc = currentSequence.PlayerChoices[0];

                    if (firstPc.ChoiceText != string.Empty)
                    {
                        PauseSequence();
                        //spawn player choice buttons, and assign accordingly
                        foreach (Player_Choice pc in currentSequence.PlayerChoices)
                        {
                            if (pc.RequireCondition && !pc.MetAllConditions()) continue; //don't display a choice if it hasn't met its conditions

                            Button b = Instantiate(playerchoicePrefab, playerchoiceHolder);
                            b.GetComponentInChildren<TextMeshProUGUI>().text = pc.ChoiceText;
                            b.onClick.AddListener(delegate { LoadASequence(pc.ResultingSequence); UnpauseSequence(); });
                        }
                    }
                    //if the string is empty for first choice, don't give buttons, and instead load first valid sequence
                    else
                    {
                        foreach(Player_Choice pc in currentSequence.PlayerChoices)
                        {
                            if(pc.RequireCondition && !pc.MetAllConditions()) continue;
                            LoadASequence(pc.ResultingSequence);
                            break;
                        }
                    }
                }
            }
        }

        private IEnumerator RunThroughScreen(Sequences.Screen screen)
        {
            bool prevActorIscurSpeaker = (curActor == screen.Speaker);
            if (curActor != null && !prevActorIscurSpeaker && currentSequence.AutoFocusSpeaker) yield return ScaleActor(false, autoFocusScaleDuration);
            //scale down in case our previous actor was scaled up, if we don't have the same actor
            foreach (Screen_Event se in screen.ScreenActions) //do all screen events BEFORE processing any dialog. does not care if SM is paused or not.
            {
                se.DoShit();
            }
            if (screen.Speaker != null) curActor = screen.Speaker; //set the current actor for reasons. why is this an if?
            else curActor = null;

            ///if(screen.Dialog == string.Empty) { jumpToEndDialog = true; ProgressToNextScreen(); yield break; }
            jumpToEndDialog = false; //set up to make sure we can skip properly and not just constantly move on before reaching end of text

            if (curActor != null) //if we have an actor, we can put a name to our dialog box, and set the appropriate colors
            {
                speakerNameBox.gameObject.SetActive(true);
                speakerNameBox.color = curActor.TextboxColor;
                dialogBox.color = curActor.TextboxColor;
                speakerNameField.text = curActor.ActorName;
                if(curActor.ActorName == playerTag) speakerNameField.text = playerName; 
                if (currentSequence.AutoFocusSpeaker && !prevActorIscurSpeaker) StartCoroutine(ScaleActor(true, autoFocusScaleDuration)); //zoom in on speaker if the bool says so
            }
            else //if no actor assigned, assume it's narration, so no name to our dialog box
            {
                speakerNameBox.color = defaultTextBoxColor;
                speakerNameBox.gameObject.SetActive(false);
                dialogBox.color = defaultTextBoxColor;
            }

            dialogField.text = ""; //wipe before putting in the new text

            float t = 0;
            int i = 0;
            string amended = Regex.Replace(screen.Dialog, playerTag, playerName); //insert the player's custom name into dialog
            char[] dialogchars = amended.ToCharArray();

            if(dialogchars.Length == 0) //hover on empty screens until the transition or whatever is finished
            {
                while (paused)
                {
                    yield return null;
                }
                //manually move on, we don't want to show off a true empty if we can't help it.
                jumpToEndDialog = true;
                ProgressToNextScreen();
                yield break;
            }
            else UpdateHistory(curActor != null ? curActor.ActorName == playerTag ? playerName : curActor.ActorName : "Internal Narration", amended);

            while (dialogchars.Length > dialogField.text.Length && amended.Length > dialogField.text.Length && !jumpToEndDialog)
            {
                //only run through text if the SM is unpaused
                while (paused) 
                {
                    yield return null;
                }

                t += Time.deltaTime;
                //add one character at a time, depending on text speed
                if (t >= curSpeed && i < dialogchars.Length)
                {
                    t = 0;

                    if (dialogchars[i] == '<') //we've found a rich text tag
                    {
                        string tag = "" + dialogchars[i];
                        while (dialogchars[i] != '>')
                        {
                            i++;
                            tag += dialogchars[i];
                        }
                        i++;

                        if (!TagParser.Parse(tag)) //if it's not a tagparser tag, it's probably unity valid. add that boy back in.
                        {
                            dialogField.text += tag;
                        }
                        else //remove tags from the final display if it's being handled by tag parser
                        {
                            amended = Regex.Replace(amended, tag, "");
                        }
                    }
                    else //just add the char and move on if it's a regular ah character
                    {
                        dialogField.text += dialogchars[i];
                        i++;
                    }
                }

                yield return null;
            }
            //safety measure
            StartCoroutine(FlashButton(curScreenIndex));
            jumpToEndDialog = true;
            dialogField.text = TagParser.CleanOutTags(amended);

            if (auto) yield return AutoProgress(curScreenIndex);
        }

        private IEnumerator AutoProgress(int index)
        {
            float t = 0;
            while(t < lingerTime)
            {
                if (index != curScreenIndex) yield break;
                t += Time.deltaTime;
                yield return null;
            }
            ProgressToNextScreen();
        }
        private void UpdateHistory(string speaker, string text)
        {
            //first, see if there's any space in the arrays
            for (int i = 0; i < curHist.HistoryLength; i++)
            {
                if (curHist.SpeakerNames[i] == null)
                {
                    curHist.SpeakerNames[i] = speaker;
                    curHist.DialogBoxes[i] = text;
                    return;
                }
            }
            //if we found no room, replace everything going down.
            for (int i = curHist.HistoryLength - 1; i >= 0; i--)
            {
                string tSpeaker = curHist.SpeakerNames[i];
                string tDialog = curHist.DialogBoxes[i];

                curHist.SpeakerNames[i] = speaker;
                curHist.DialogBoxes[i] = text;

                if (i == 0) return; //don't bother spending cpu if we are at the final operation.

                speaker = tSpeaker;
                text = tDialog;
            }
        }
        #endregion
        #region SaveLoadHandling
        public void SaveShit(string saveFileName)
        {
            if (saving) return;

            if(overlayDatabase == null || backgroundDatabase == null || audioDatabase == null)
            {
                Debug.LogWarning("You're a missing a database, you damned fool! I refuse to save under these working conditions!");
                return;
            }
            saving = true;

            SaveData manToSave = new SaveData();
            ScreenToken st = new ScreenToken();

            manToSave.CurrentScreenIndex = curScreenIndex; //:)
            manToSave.PlayerName = playerName;
            manToSave.CurrentFlags = new FlagToken(Flag_Manager.GetSaveDataFlags);
            manToSave.CurrentHistory = curHist;

            //grab the cursequence. horrid! USES THE ASSET REFERENcE WE DONE STORED. MAYBE IT WORK? MAYBE IT NO :)
            manToSave.CurrentSequenceAsset = currentSequence.Myself.AssetGUID;

            if (Object_Factory.TryGetObject("Background", out GameObject bg)) 
            {
                Image image = bg.GetComponent<Image>();
                st.BackgroundAssetIndex = backgroundDatabase.Sprites.IndexOf(image.sprite);
            }

            if(Object_Factory.TryGetObject("Overlay", out GameObject ov))
            {
                Image image = ov.GetComponent<Image>();
                st.OverlayAssetIndex = overlayDatabase.Sprites.IndexOf(image.sprite);
            }

            st.MusicAssetIndex = audioDatabase.Sounds.IndexOf(Audio_Manager.AM.CurrentBGM);

            List<ActorToken> actorTokens = new();

            foreach (Actor actor in activeActors)
            {
                if (Object_Factory.TryGetObject(actor.ActorName, out GameObject go))
                {
                    ActorToken newt = new();
                    UI_Element uie = go.GetComponent<UI_Element>();
                    newt.XPos = go.transform.position.x;
                    newt.YPos = go.transform.position.y;
                    newt.ZPos = go.transform.position.z;
                    List<int> visualIndexes = new();
                    for (int i = 0; i < uie.Images.Length; i++)
                    {
                        visualIndexes.Add(actor.Visuals[i].layer.IndexOf(uie.Images[i].sprite)); //HIDEOUS
                    }
                    newt.VisualIndexes = visualIndexes.ToArray();
                    newt.ActorAsset = actor.Myself.AssetGUID;
                    Debug.Log("ActorToken I'm adding to list: \n" + newt.ToString());
                    actorTokens.Add(newt);
                }
            }

            st.ActiveActors = actorTokens;
            manToSave.ScreenInformation = st;
            menuBase.SetActive(false);
            StartCoroutine(WaitForScreenShot(manToSave, saveFileName));
            SaveLoad.Save(saveFileName, manToSave);
        }

        private IEnumerator WaitForScreenShot(SaveData sd, string fileName)
        {
            yield return new WaitForEndOfFrame();
            sd.SaveScreenshot = ScreenCapture.CaptureScreenshotAsTexture().EncodeToPNG();
            menuBase.SetActive(true);
            SaveLoad.Save(fileName, sd);
            saving = false;
        }

        public void LoadShit(SaveData sd)
        {
            //wipe the brown poops
            StopAllCoroutines();
            Object_Factory.ScrubDictionary();

            Image ov = Object_Factory.SpawnObject(overlayPrefab, "Overlay", overlayHolder).GetComponent<Image>();
            Image bg = Object_Factory.SpawnObject(overlayPrefab, "Background", GameObject.Find("BGcanv").transform).GetComponent<Image>();

            //grab flags
            FlagToken ft = sd.CurrentFlags;
            Flag_Manager.ReceiveFlagToken(ft, false);

            //grab history
            curHist = sd.CurrentHistory;

            //grab playername from file
            playerName = sd.PlayerName;

            //grab assets
            curScreenIndex = sd.CurrentScreenIndex;
            ScreenToken std = sd.ScreenInformation;
            AsyncOperationHandle SequenceAsset;

            ov.sprite = overlayDatabase.Sprites[std.OverlayAssetIndex];
            bg.sprite = backgroundDatabase.Sprites[std.BackgroundAssetIndex];

            Audio_Manager.AM.PlayBGM(audioDatabase.Sounds[std.MusicAssetIndex], 1f, true, 
               Audio_Manager.AM.CurrentBGM == audioDatabase.Sounds[std.MusicAssetIndex] ? true : false);

            SequenceAsset = Addressables.LoadAssetAsync<Sequence>(sd.CurrentSequenceAsset);

            Debug.Log("Amount of actors we should be loading: " + std.ActiveActors.Count);

            foreach (ActorToken at in std.ActiveActors) //spawn in all of the actors that were chillin' like villain before
            {
                Debug.Log("Loading an actor:\n" + at.ToString());
                AsyncOperationHandle ActorSO = Addressables.LoadAssetAsync<Actor>(at.ActorAsset);
                ActorSO.WaitForCompletion();
                if (ActorSO.Status == AsyncOperationStatus.Succeeded)
                {
                    Actor guy = (Actor)ActorSO.Result;
                    GameObject go = Object_Factory.SpawnObject(guy.ActorPrefab, guy.ActorName, actorHolder);
                    if (go == null) continue;
                    UI_Element uie = go.GetComponent<UI_Element>();
                    for (int i = 0; i < at.VisualIndexes.Length; i++)
                    {
                        uie.Images[i].sprite = guy.Visuals[i].layer[at.VisualIndexes[i]];
                    }
                    uie.transform.position = new Vector3(at.XPos, at.YPos, at.ZPos);
                }
                else
                {
                    Debug.LogWarning("Failed to load actor: " + at.ActorAsset);
                    ActorSO.Release();
                }
            }

            SequenceAsset.WaitForCompletion();
            if (SequenceAsset.Status == AsyncOperationStatus.Succeeded) currentSequence = (Sequence)SequenceAsset.Result;
            else SequenceAsset.Release();
            loaded = true;
            StartCoroutine(RunThroughScreen(currentSequence.Screens[curScreenIndex]));
        }
        #endregion
        private IEnumerator ScaleActor(bool up, float scaleTime) //used if autoSpeakerFocus is true in a sequence
        {
            float t;
            float eval;
            if (!Object_Factory.TryGetObject(curActor.ActorName, out GameObject fella))
            {
                yield break; //break out of routine if we can't find the gameobject we want
            }

            if (up)
            {
                fella.transform.SetAsLastSibling(); //move to the front of the bus
                t = 0;
                while (t < scaleTime)
                {
                    if (fella == null) yield break;
                    t += Time.deltaTime;
                    eval = actorScalingKurve.Evaluate(t / scaleTime);
                    fella.transform.localScale = new Vector3(eval, eval, eval);
                    yield return null;
                }
            }
            else
            {
                fella.transform.SetAsFirstSibling(); //move to the back of the bus
                t = scaleTime;
                while (t > 0)
                {
                    if (fella == null) yield break;
                    t -= Time.deltaTime;
                    eval = actorScalingKurve.Evaluate(t / scaleTime);
                    fella.transform.localScale = new Vector3(eval, eval, eval);
                    yield return null;
                }
            }
        }

        private IEnumerator FlashButton(int scrindex)
        {
            Color slightlyTrans = new Color(1, 1, 1, 0.4f);
            float t = 0;
            bool dir = true;
            Image pd = progressDialogButton.GetComponent<Image>();

            while (scrindex == curScreenIndex) 
            {
                t += dir ? Time.deltaTime : Time.deltaTime * -1;

                if (t >= 1) dir = false;
                else if (t <= 0) dir = true;

                pd.color = Color.Lerp(Color.white, slightlyTrans, t);

                yield return null;
            }

            pd.color = Color.white;
        }

        private void ToggleDialogUI(bool b)
        {
            dialogBox.transform.parent.gameObject.SetActive(!b);
        }

        public void FlipAuto(bool b)
        {
            auto = b;
            toggleImage.color = b ? togglePressedColor : Color.white;

            if (b && jumpToEndDialog) ProgressToNextScreen();
        }

        public void SetSpeed(float value, bool reset = false)
        {
            if (reset) curSpeed = textSpeed;
            else curSpeed = value;
        }
        #region Settings
        private void TextSpeed(float f)
        {
            textSpeed = f;
            curSpeed = textSpeed;
        }
        private void TextAutoHang(float f)
        {
            lingerTime = f;
        }
        private void SetPlayerName(string s)
        {
            playerName = s;
        }
        #endregion
    }
}
