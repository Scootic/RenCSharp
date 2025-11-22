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
        [Header("Animate Overlay")]
        [SerializeField] private bool animate = false;
        [SerializeField, Tooltip("Be careful if you set this to false. Animations will loop until you override them.")] private bool endWithScreen = true;
        [SerializeField, Min(0), Tooltip("0 for every frame.")] private float secondsPerFrame = 0.1f;
        private Coroutine animation;
        private bool bloop;

        public override void DoShit()
        {
            Image overlay = GameObject.Find("Overlay").GetComponent<Image>();
            overlay.sprite = imagesToSet[0];
            if(animate)
            {
                bloop = true;
                animation = Script_Manager.SM.StartCoroutine(AnimateOverlay(overlay, imagesToSet));
                if (endWithScreen) Script_Manager.ProgressScreenEvent += PanicStop;
            }
            GameObject.Find("OverlayText").GetComponent<TextMeshProUGUI>().text = overlayText;
        }

        private void PanicStop()
        {
            bloop = false;
            Script_Manager.SM.StopCoroutine(animation);
        }

        private IEnumerator AnimateOverlay(Image overlay, List<Sprite> sprites)
        {
            float t = 0;
            int i = 0;
            while (bloop)
            {
                if (!sprites.Contains(overlay.sprite)) { PanicStop(); yield break; }
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
