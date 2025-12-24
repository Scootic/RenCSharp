using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace RenCSharp.Sequences
{
    public class Set_Overlay : Screen_Event
    {
        [SerializeField, Tooltip("If not animated, just uses index 0.")] private List<Sprite> imagesToSet;
        [SerializeField, Tooltip("Title card type stuff.")] private string overlayText = string.Empty;
        [SerializeField] private float fadeTime = 0.5f;
        [Header("Animate Overlay")]
        [SerializeField, Tooltip("Be careful if you set this to false. Animations will loop until you override them.")] private bool endWithScreen = true;
        [SerializeField, Min(0), Tooltip("0 for every frame.")] private float secondsPerFrame = 0.1f;

        private Coroutine fadeImage;
        private Animated_Image_Handler overlay;

        public override void DoShit()
        {
            if (!Object_Factory.TryGetObject("Overlay", out GameObject go)) return;
            overlay = go.GetComponent<Animated_Image_Handler>();
            fadeImage = Script_Manager.SM.StartCoroutine(FadeIn(overlay.Image, imagesToSet));
            if (endWithScreen) Script_Manager.ProgressScreenEvent += PanicStop;
        }

        private void PanicStop()
        {
            Debug.LogWarning("Set overlay panic stopped!");
            overlay.Image.color = Color.white;
            overlay.ReceiveAnimationInformation(imagesToSet.ToArray(), secondsPerFrame);
            if (fadeImage != null)Script_Manager.SM.StopCoroutine(fadeImage);
        }

        private IEnumerator FadeIn(Image overlayImg, List<Sprite> sprites)
        {
            float t = 0;
            Color transGender = new Color(1, 1, 1, 0);
            float perc;
            bool flick = false;
            TextMeshProUGUI text = overlay.GetComponent<UI_Element>().Texts[0];
            text.text = "";
            while (t <= fadeTime)
            {
                t += Time.deltaTime;
                perc = t / fadeTime;

                if(perc < 0.5f)
                {
                    overlayImg.color = Color.Lerp(Color.white, transGender, perc * 2);
                }
                else
                {
                    if (!flick) 
                    {
                        overlay.ReceiveAnimationInformation(sprites.ToArray(), secondsPerFrame);
                        flick = true;
                    }

                    overlayImg.color = Color.Lerp(transGender, Color.white, perc * 2 - 1);
                }

                yield return null;
            }
            text.text = overlayText;
            overlayImg.color = Color.white;
        }

        public override string ToString()
        {
            return "Set Overlay Image";
        }
    }
}
