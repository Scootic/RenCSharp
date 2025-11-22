using UnityEngine;
using RenCSharp.Sequences;
using RenCSharp.Actors;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
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
        [SerializeField] private Button playerchoicePrefab;
        [SerializeField] private Button progressDialogButton;
        [SerializeField] private Transform playerchoiceHolder;
        [SerializeField, Tooltip("Decides the color of the textboxes if there's no actor in a screen.")] private Color defaultTextBoxColor = Color.white;

        [Header("Actors")]
        [SerializeField] private Transform actorHolder;
        [SerializeField] private AnimationCurve actorScalingKurve;
        private Actor curActor;

        [Header("Settings")]
        [SerializeField, Tooltip("In seconds, 0 for character every frame."), Min(0)] private float textSpeed = 0;
        [SerializeField, Tooltip("In seconds."), Min(0)] private float autoFocusScaleDuration = 0.25f;
        [SerializeField] private string playerName = "Guy"; //probably should be handled by an save data
        [SerializeField, Tooltip("This will be string that is replaced by inputted player name.")] private string playerTag = "{MC}";

        private bool jumpToEndDialog = false;
        private bool paused = false;
        private History curHist;
        private Dictionary<string, int> curFlags;

        public static Script_Manager SM;
        public static Action ProgressScreenEvent, EndOfAllSequencesEvent;
        public static Action<bool> SequencePausedEvent;
        public Transform ActorHolder => actorHolder;
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

            curFlags = new Dictionary<string, int>(); //load save data if we found it?
            curHist = new History();

            DontDestroyOnLoad(gameObject); //might be upset when loading scenes and losing references to dialog boxes, etc.
            SequencePausedEvent += SetButtonInteractable;
        }

        void Start()
        {
            StartSequence();
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
            if (paused) return; //don't go nowhere if the SM is paused
            if (!jumpToEndDialog) jumpToEndDialog = true;
            else
            {
                Debug.Log("Moving to next screen!");
                ProgressScreenEvent?.Invoke();
                ProgressScreenEvent = null; //wipe all delegates from the action before continuing
                curScreenIndex++;
                Debug.Log("current Scrindex: " + curScreenIndex + ", Final Screen? " + (curScreenIndex >= currentSequence.Screens.Length - 1));
                if(curScreenIndex < currentSequence.Screens.Length) StartCoroutine(RunThroughScreen(currentSequence.Screens[curScreenIndex]));
                if(curScreenIndex > currentSequence.Screens.Length - 1)//final screen of the sequence
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
                        //spawn player choice buttons, and assign accordingly
                        foreach (Player_Choice pc in currentSequence.PlayerChoices)
                        {
                            if (pc.RequireCondition && !pc.MetAllConditions()) continue; //don't display a choice if it hasn't met its conditions

                            Button b = Instantiate(playerchoicePrefab, playerchoiceHolder);
                            b.GetComponentInChildren<TextMeshProUGUI>().text = pc.ChoiceText;
                            b.onClick.AddListener(delegate { LoadASequence(pc.ResultingSequence); });
                        }
                    }
                    //if the first choice has no choice text, and conditionallity is good, do that shit.
                    else if (!firstPc.RequireCondition || firstPc.MetAllConditions()) 
                    {
                        LoadASequence(currentSequence.PlayerChoices[0].ResultingSequence);
                    }
                }
            }
        }

        private IEnumerator RunThroughScreen(Sequences.Screen screen)
        { 
            if (curActor != null && curActor != screen.Speaker) yield return ScaleActor(false, autoFocusScaleDuration);
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
                if (currentSequence.AutoFocusSpeaker) StartCoroutine(ScaleActor(true, autoFocusScaleDuration)); //zoom in on speaker if the bool says so
            }
            else //if no actor assigned, assume it's narration, so no name to our dialog box
            {
                speakerNameBox.color = defaultTextBoxColor;
                speakerNameBox.gameObject.SetActive(false);
                dialogBox.color = defaultTextBoxColor;
            }

            dialogField.text = ""; //wipe before putting in the new text

            if (screen.Dialog == "")
            {
                Debug.Log("No dialog on this screen, moving to next!");
                jumpToEndDialog = true;
                ProgressToNextScreen();
                yield break;
            }

            float t = 0;
            int i = 0;
            string amended = Regex.Replace(screen.Dialog, playerTag, playerName); //insert the player's custom name into dialog
            char[] dialogchars = amended.ToCharArray();

            while (dialogchars.Length > dialogField.text.Length && !jumpToEndDialog)
            {
                //only run through text if the SM is unpaused
                if (!paused)
                {
                    t += Time.deltaTime;
                    //add one character at a time, depending on text speed
                    if (t >= textSpeed)
                    {
                        t = 0;
                        dialogField.text += dialogchars[i];
                        i++;
                    }
                }
                yield return null;
            }
            //safety measure
            jumpToEndDialog = true;
            dialogField.text = amended;
        }
        #endregion
        #region FlagHandling
        public void SetFlag(string id, int val)
        {
            Debug.Log("Setting flag: " + id + ", to: " + val);
            if (curFlags.ContainsKey(id)) curFlags[id] = val;
            else curFlags.Add(id, val);
        }
        //defaults to negative one if there's no id in the dictionary, please be careful of manually setting and checking negative values.
        public int GetFlag(string id)
        {
            int val = -1;

            if (curFlags.ContainsKey(id)) val = curFlags[id];

            return val;
        }
        public void IncrementFlag(string id, int valToIncreaseBy)
        {
            Debug.Log("Incrementing flag: " + id + ", increasing by: " + valToIncreaseBy);
            if (curFlags.ContainsKey(id)) curFlags[id] += valToIncreaseBy;
            else curFlags.Add(id, valToIncreaseBy);
        }
        #endregion
        private IEnumerator ScaleActor(bool up, float scaleTime) //used if autoSpeakerFocus is true in a sequence
        {
            float t;
            float eval;
            GameObject fella = GameObject.Find(curActor.ActorName);
            if (fella == null)
            {
                Debug.Log("could find gameobject for: " + curActor.ActorName); 
                yield break; //break out of routine if we can't find the gameobject we want
            }

            if (up)
            {
                fella.transform.SetAsLastSibling(); //move to the front of the bus
                t = 0;
                while (t < scaleTime)
                {
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
                    t -= Time.deltaTime;
                    eval = actorScalingKurve.Evaluate(t / scaleTime);
                    fella.transform.localScale = new Vector3(eval, eval, eval);
                    yield return null;
                }
            }
        } 

        private void SetButtonInteractable(bool b)
        {
            progressDialogButton.interactable = !b;
        }

        private void UpdateHistory()
        {
            //not implemented
        }
    }
}
