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

        [Header("Settings")]
        [SerializeField, Tooltip("In seconds, 0 for character every frame?")] private float textSpeed = 0;

        private bool jumpToEndDialog = false;

        public static Script_Manager SM;
        //certified singleton moment
        private void Awake()
        {
            if(SM == null)
            {
                SM = this;
            }else if(SM != this)
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
            if(!jumpToEndDialog) jumpToEndDialog = true;
            else
            {
                curScreenIndex++;
                if (curScreenIndex < currentSequence.Screens.Length) StartCoroutine(RunThroughScreen(currentSequence.Screens[curScreenIndex]));
            }
        }

        private IEnumerator RunThroughScreen(RenCSharp.Sequences.Screen screen)
        {
            jumpToEndDialog = false;
            speakerNameField.text = screen.Speaker.ActorName;
            dialogField.text = "";
            float t = 0;
            int i = 0;
            //do screen events first when I figure that shit out

            char[] dialogchars = screen.Dialog.ToCharArray();

            while(dialogchars.Length > dialogField.text.Length && !jumpToEndDialog)
            {
                t += Time.deltaTime;

                if(t >= textSpeed)
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
    }
}
