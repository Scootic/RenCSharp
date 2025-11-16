using UnityEngine;
using RenCSharp.Sequences;
using RenCSharp.Actors;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Text.RegularExpressions;
using System;
namespace RenCSharp
{
    /// <summary>
    /// The guy who handles the game logic. Runs through the screens of a sequence, and does all the stuff.
    /// </summary>
    public class Script_Manager : MonoBehaviour
    {
        [SerializeField] private Sequence currentSequence; //set before runtime for first sequence of unity scene
        private int curScreenIndex = 0;

        [Header("Dialog Fields")]
        [SerializeField] private TextMeshProUGUI speakerNameField;
        [SerializeField] private TextMeshProUGUI dialogField;
        [SerializeField] private Image speakerNameBox;
        [SerializeField] private Image dialogBox;
        [SerializeField] private Button playerchoicePrefab;
        [SerializeField] private Transform playerchoiceHolder;

        [Header("Actors")]
        [SerializeField] private Transform[] actorPositions;
        [SerializeField] private AnimationCurve actorScalingKurve;
        private Actor curActor;

        [Header("Settings")]
        [SerializeField, Tooltip("In seconds, 0 for character every frame?")] private float textSpeed = 0;
        [SerializeField] private string playerName = "Guy";

        private bool jumpToEndDialog = false;

        public static Script_Manager SM;
        public static Action ProgressScreenEvent;
        public Transform[] ActorPositions => actorPositions;
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

            DontDestroyOnLoad(gameObject); //might be upset when loading scenes and losing references to dialog boxes, etc.
        }

        void Start()
        {
            StartSequence();
        }

        public void StartSequence()
        {
            StartCoroutine(RunThroughScreen(currentSequence.Screens[0]));
        }

        public void ProgressToNextScreen() //for an UI button to use
        {
            if (!jumpToEndDialog) jumpToEndDialog = true;
            else
            {
                ProgressScreenEvent?.Invoke();
                ProgressScreenEvent = null; //wipe all delegates from the action before continuing
                curScreenIndex++;
                if (curScreenIndex < currentSequence.Screens.Length - 1) StartCoroutine(RunThroughScreen(currentSequence.Screens[curScreenIndex]));
                else //final screen of the sequence
                {
                    if (currentSequence.PlayerChoices.Length == 0) { Debug.Log("No next sequence, game over?"); return; }

                    if (currentSequence.PlayerChoices[0].ChoiceText != string.Empty)
                    {
                        //spawn player choice buttons, and assign accordingly
                        foreach (Player_Choice pc in currentSequence.PlayerChoices)
                        {
                            Button b = Instantiate(playerchoicePrefab, playerchoiceHolder);
                            b.onClick.AddListener(delegate { LoadASequence(pc.ResultingSequence); });
                        }
                    }
                    else //if your first choice contains no text for the button, assume that there's no choice and load next sequence
                    {
                        LoadASequence(currentSequence.PlayerChoices[0].ResultingSequence);
                    }
                }
            }
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

        private IEnumerator RunThroughScreen(Sequences.Screen screen)
        { 
            if (curActor != null) yield return ScaleActor(false); //scale down in case our previous actor was scaled up
            foreach (Screen_Event se in screen.ScreenActions) //do all screen events BEFORE processing any dialog
            {
                se.DoShit();
            }

            curActor = screen.Speaker; //set the current actor for reasons
            jumpToEndDialog = false; //set up to make sure we can skip properly and not just constantly move on before reaching end of text

            if (curActor != null) //if we have an actor, we can put a name to our dialog box
            {
                speakerNameBox.gameObject.SetActive(true);
                speakerNameField.text = curActor.ActorName;
                if (currentSequence.AutoFocusSpeaker) StartCoroutine(ScaleActor(true)); //zoom in on speaker if the bool says so, may be a layering moment
            }
            else //if no actor assigned, assume it's narration, so no name to our dialog box
            {
                speakerNameBox.gameObject.SetActive(false);
            }

            dialogField.text = ""; //wipe before putting in new wordses
            float t = 0;
            int i = 0;
            string amended = Regex.Replace(screen.Dialog, "{MC}", playerName); //insert the player's custom name into dialog
            char[] dialogchars = amended.ToCharArray();

            while (dialogchars.Length > dialogField.text.Length && !jumpToEndDialog)
            {
                t += Time.deltaTime;
                //add one character at a time, depending on text speed
                if (t >= textSpeed)
                {
                    t = 0;
                    dialogField.text += dialogchars[i];
                    i++;
                }
                yield return null;
            }
            //safety measure
            dialogField.text = screen.Dialog;
        }
   

        private IEnumerator ScaleActor(bool up)
        {
            float t;
            float eval;
            GameObject fella = GameObject.Find(curActor.ActorName);
            if (fella == null) yield break; //break out of routine if we can't find the gameobject we want

            if (up)
            {
                t = 0;
                while (t < 1)
                {
                    t += Time.deltaTime;
                    eval = actorScalingKurve.Evaluate(t);
                    fella.transform.localScale = new Vector3(eval, eval, eval);
                    yield return null;
                }
            }
            else
            {
                t = 1;
                while (t > 0)
                {
                    t -= Time.deltaTime;
                    eval = actorScalingKurve.Evaluate(t);
                    fella.transform.localScale = new Vector3(eval, eval, eval);
                    yield return null;
                }
            }
        } 
    }
}
