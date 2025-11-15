using UnityEngine;
using RenCSharp.Sequences;
using RenCSharp.Actors;
using UnityEngine.UI;
using TMPro;
using System.Collections;
namespace RenCSharp
{
    public class Script_Manager : MonoBehaviour
    {
        [SerializeField] private Sequence currentSequence; //set before runtime for first sequence of scene
        private int curScreenIndex = 0;

        [Header("Dialog Fields")]
        [SerializeField] private TextMeshProUGUI speakerNameField;
        [SerializeField] private TextMeshProUGUI dialogField;
        [SerializeField] private Image speakerNameBox;
        [SerializeField] private Image dialogBox;

        [Header("Actors")]
        [SerializeField] private Transform[] actorPositions;
        [SerializeField] private AnimationCurve actorScalingKurve;
        private Actor curActor;

        [Header("Settings")]
        [SerializeField, Tooltip("In seconds, 0 for character every frame?")] private float textSpeed = 0;

        private bool jumpToEndDialog = false;

        public static Script_Manager SM;
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
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartSequence();
        }

        // Update is called once per frame
        void StartSequence()
        {
            StartCoroutine(RunThroughScreen(currentSequence.Screens[0]));
        }

        public void ProgressToNextScreen()
        {
            if (!jumpToEndDialog) jumpToEndDialog = true;
            else
            {
                curScreenIndex++;
                if (curScreenIndex < currentSequence.Screens.Length) StartCoroutine(RunThroughScreen(currentSequence.Screens[curScreenIndex]));
            }
        }

        private IEnumerator RunThroughScreen(RenCSharp.Sequences.Screen screen)
        {
            if(curActor != null) yield return ScaleActor(false);
            curActor = screen.Speaker;
            jumpToEndDialog = false;
            if (curActor != null)
            {
                speakerNameBox.gameObject.SetActive(true);
                speakerNameField.text = curActor.ActorName;
                if (currentSequence.AutoFocusSpeaker) StartCoroutine(ScaleActor(true));
            }
            else
            {
                speakerNameBox.gameObject.SetActive(false);
            }
            dialogField.text = "";
            float t = 0;
            int i = 0;
            //do screen events first when I figure that shit out

            foreach(Screen_Event se in screen.ScreenActions)
            {
                se.DoShit();
            }

            char[] dialogchars = screen.Dialog.ToCharArray();

            while (dialogchars.Length > dialogField.text.Length && !jumpToEndDialog)
            {
                t += Time.deltaTime;

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
            if (fella == null) yield break;
            
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
