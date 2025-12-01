
using UnityEngine;
using UnityEngine.UI;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// Screen Event to change the background to something else, with a fade to hide it goodly better.
    /// Technically problematic; remember to assign your index 0 anim events as the middle of a fade and your index 1 anim events as
    /// the end of a fade.
    /// </summary>
    public class Fade_Transition : Screen_Event
    {
        [SerializeField] private Sprite newBG;
        [SerializeField, Tooltip("Decide which type of transition to tell animator to use.")] private int fadeTransition = 0;
        [SerializeField, Tooltip("How long should the fade be in seconds?")] private float fadeDuration = 1f;
        private Animation_Event_Delegates aed;
        public override void DoShit()
        {
            if (GameObject.FindGameObjectWithTag("Fader").TryGetComponent(out Animator fader)) //find the fader
            {
                fader.SetInteger("FadeType", fadeTransition);
                fader.SetTrigger("Fade");
                fader.SetFloat("SpeedMult", 1f / fadeDuration);
                fader.Update(0.01f); //make sure the animator is actually in the state we want, and not just the empty idle state.

                if (fader.TryGetComponent(out aed)) //if we can't find an aed, don't perma stun the sequence forever
                {
                    Script_Manager.SM.PauseSequence(); //pause to prevent things happening during fade transition
                    aed.AnimationDelegates[0] += SwapBG;
                    aed.AnimationDelegates[1] += UnpauseSM;

                    Script_Manager.ProgressScreenEvent += delegate { aed.WipeDelegates(); };
                }
            }
            else
            {
                Debug.LogWarning("couldn't find a fader gameobject. Did you set it to be tag: 'Fader'?");
            }
        }
        //?
        private void SwapBG()
        {
            if (!Object_Factory.TryGetObject("Background", out GameObject go)) return;
            go.GetComponent<Image>().sprite = newBG;
        }

        private void UnpauseSM()
        {
            Script_Manager.SM.UnpauseSequence();
        }

        public override string ToString()
        {
            return "Fade Transition";
        }
    }
}
