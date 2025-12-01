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
        [SerializeField] private bool animate = false;
        [SerializeField, Tooltip("Be careful if you set this to false. Animations will loop until you override them.")] private bool endWithScreen = true;
        [SerializeField, Min(0), Tooltip("0 for every frame.")] private float secondsPerFrame = 0.1f;

        private Coroutine animation;
        private Coroutine fadeImage;
        private bool bloop;
        private Image overlay;

        public override void DoShit()
        {
            if (!Object_Factory.TryGetObject("Overlay", out GameObject go)) return;
            overlay = go.GetComponent<Image>();
            bloop = true;
            fadeImage = Script_Manager.SM.StartCoroutine(FadeIn(overlay, imagesToSet));
            if (endWithScreen && animate) Script_Manager.ProgressScreenEvent += PanicStop;
        }

        private void PanicStop()
        {
            Debug.LogWarning("Set overlay panic stopped!");
            bloop = false;
            overlay.color = Color.white;
            if (fadeImage != null)Script_Manager.SM.StopCoroutine(fadeImage);
            if(animation != null)Script_Manager.SM.StopCoroutine(animation);
        }

        private IEnumerator FadeIn(Image overlay, List<Sprite> sprites)
        {
            float t = 0;
            Color transGender = new Color(1, 1, 1, 0);
            float perc;
            bool flick = false;
            TextMeshProUGUI text = overlay.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            text.text = "";
            while (t <= fadeTime)
            {
                t += Time.deltaTime;
                perc = t / fadeTime;

                if(perc < 0.5f)
                {
                    overlay.color = Color.Lerp(Color.white, transGender, perc * 2);
                }
                else
                {
                    if (!flick) 
                    {
                        if (!animate) overlay.sprite = sprites[0];
                        else animation = Script_Manager.SM.StartCoroutine(AnimateOverlay(overlay, sprites));
                        flick = true;
                    }

                    overlay.color = Color.Lerp(transGender, Color.white, perc * 2 - 1);
                }

                yield return null;
            }
            text.text = overlayText;
            overlay.color = Color.white;
        }

        private IEnumerator AnimateOverlay(Image overlay, List<Sprite> sprites)
        {
            float t = 0;
            int i = 0;
            Sprite ogSp = overlay.sprite;

            while (bloop)
            {
                if (!sprites.Contains(overlay.sprite) && ogSp != overlay.sprite) { PanicStop(); yield break; }
                t += Time.deltaTime;
                if(t >= secondsPerFrame)
                {
                    t = 0;
                    i++;
                    if (i == sprites.Count) i = 0;
                    overlay.sprite = sprites[i];
                }
                yield return null;
            }
        }

        public override string ToString()
        {
            return "Set Overlay Image";
        }
    }
}
