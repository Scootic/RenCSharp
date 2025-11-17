using Codice.Client.BaseCommands.CheckIn;
using UnityEngine;
using UnityEngine.UI;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// Screen Event to change the background to something else, with a fade to hide it goodly better.
    /// </summary>
    public class Fade_Transition : Screen_Event
    {
        [SerializeField] private Sprite newBG;
        [SerializeField, Tooltip("Decide which type of transition to tell animator to use.")] private int fadeTransition = 0;
        [SerializeField, Tooltip("How long should the fade be in seconds?")] private float fadeDuration = 1f;
        public override void DoShit()
        {
            Image bg = GameObject.Find("BG").GetComponent<Image>(); //probably shouldn't be this

            if (GameObject.FindGameObjectWithTag("Fader").TryGetComponent(out Animator fader)) //find the fader
            {
                Script_Manager.SM.PauseSequence();
                fader.SetInteger("FadeType", fadeTransition);
                fader.SetTrigger("Fade");
                fader.SetFloat("SpeedMult", 1f / fadeDuration);
                AnimationClip transition = fader.GetCurrentAnimatorClipInfo(0)[fadeTransition + 1].clip;
                Debug.Log(transition.name); //hopefully, should be right one!

                AnimationEvent swapBG = new AnimationEvent();
                swapBG.functionName = "SwapBG";
                AnimationEvent unpauseSM = new AnimationEvent();
                unpauseSM.functionName = "UnpauseSM";

                transition.events[0] = swapBG;
                transition.events[1] = unpauseSM;
            }
        }

        public void SwapBG()
        {
            GameObject.Find("BG").GetComponent<Image>().sprite = newBG;
        }

        public void UnpauseSM()
        {
            Script_Manager.SM.UnpauseSequence();
        }

        public override string ToString()
        {
            return "Fade Transition";
        }
    }
}
