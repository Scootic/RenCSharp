using UnityEngine;
using UnityEngine.UI;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// Screen Event to change the background to something else.
    /// </summary>
    public class Change_BG : Screen_Event
    {
        [SerializeField] private Sprite newBG;
        [SerializeField, Tooltip("Decide which type of transition to use. 0 for none.")] private int fadeTransition = 0;
        [SerializeField, Tooltip("Unused if fadeTransition is 0")] private float fadeDuration = 1f;
        public override void DoShit()
        {
            Image bg = GameObject.Find("BG").GetComponent<Image>();

            if(fadeTransition == 0) bg.sprite = newBG;
            else
            {
                //some coroutine bullshit
            }
        }

        public override string ToString()
        {
            return "Change Background";
        }
    }
}
