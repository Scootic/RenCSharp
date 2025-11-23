using UnityEngine;
using RenCSharp.Actors;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// Removes an actor from the scene based on a passed in Actor SO.
    /// </summary>
    [Serializable]
    public class Remove_Actor: Screen_Event
    {
        [SerializeField] private Actor actorToRemove;
        [SerializeField, Tooltip("How long it takes for the actor to fade out.")] private float fadeTime = 0f;
        private Coroutine fadeOut;
        private GameObject fellaToRemove;
        public override void DoShit()
        {
            List<Image> imgPo = new();
            fellaToRemove = GameObject.Find(actorToRemove.ActorName);
            Image img = fellaToRemove.transform.GetChild(0).GetComponent<Image>();
            imgPo.Add(img);
            for (int i = 1; i < actorToRemove.Visuals.Length; i++)
            {
                img = img.transform.GetChild(0).GetComponent<Image>();
                imgPo.Add(img);
            }

            if (fellaToRemove != null)
            {
                fadeOut = Script_Manager.SM.StartCoroutine(FadeOut(imgPo));
                Script_Manager.ProgressScreenEvent += PanicStop;
            }
            else
            {
                Debug.LogWarning("Did not find actor: " + actorToRemove.ActorName);
            }
        }

        private IEnumerator FadeOut(List<Image> imgPo)
        {
            float t = 0;
            Color transGender = new Color(0, 0, 0, 0);

            while(t < fadeTime)
            {
                t += Time.deltaTime;
                Color tcol = Color.Lerp(Color.white, transGender, (t / fadeTime));
                foreach(Image ing in imgPo)
                {
                    ing.color = tcol;
                }
                yield return null;
            }
            GameObject.Destroy(fellaToRemove);
        }

        private void PanicStop()
        {
            if(fadeOut != null) Script_Manager.SM.StopCoroutine(fadeOut);
            if (fellaToRemove != null) GameObject.Destroy(fellaToRemove);
        }

        public override string ToString()
        {
            return "Remove Actor";
        }
    }
}
