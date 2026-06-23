using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
namespace RenCSharp.Sequences
{
    public class Fade_TransitionAsset : Screen_Event
    {
        [SerializeField] private List<AssetReferenceSprite> newBG;
        [SerializeField] private float secondsPerFrame = 0.1f;
        [SerializeField, Tooltip("Decide which type of transition to tell animator to use.")] private int fadeTransition = 0;
        [SerializeField, Tooltip("How long should the fade be in seconds?")] private float fadeDuration = 1f;
        private Animation_Event_Delegates aed;
        private string[] assetRefGUIDs;
        private string[] subObjectGUIDS;

        public List<AssetReferenceSprite> SetNewBG { set { newBG = value; } }
        public float SetSecondsPerFrame { set { secondsPerFrame = value; } }
        public int SetFadeTransition { set { fadeTransition = value; } }
        public float SetFadeDuration { set { fadeDuration = value; } }

        public override void DoEvent()
        {
            if (GameObject.FindGameObjectWithTag("Fader").TryGetComponent(out Animator fader)) //find the fader
            {
                assetRefGUIDs = new string[newBG.Count];
                subObjectGUIDS = new string[newBG.Count];
                for (int i = 0; i < newBG.Count; i++)
                {
                    assetRefGUIDs[i] = newBG[i].AssetGUID;
                    subObjectGUIDS[i] = newBG[i].SubObjectName;
                }

                fader.SetInteger("FadeType", fadeTransition);
                fader.SetTrigger("Fade");
                fader.SetFloat("SpeedMult", 1f / fadeDuration);
                fader.Update(0.01f); //make sure the animator is actually in the state we want, and not just the empty idle state.

                if (fader.TryGetComponent(out aed)) //if we can't find an aed, don't perma stun the sequence forever
                {
                    Script_Manager.SM.PauseSequence(); //pause to prevent things happening during fade transition
                    Textbox_String.PauseTextbox(true);
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
            go.GetComponent<Animated_Image_Handler>().ReceiveAnimationInformation(assetRefGUIDs, subObjectGUIDS, secondsPerFrame);
        }

        private void UnpauseSM()
        {
            Textbox_String.PauseTextbox(false);
            Script_Manager.SM.UnpauseSequence();
        }

        public override string ToString()
        {
            return "Scene/Fade Transition";
        }
    }
}
