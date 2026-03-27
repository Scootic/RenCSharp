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
        [SerializeField, Tooltip("How long it takes for the actor to fade out."), Min(0f)] private float fadeTime = 0.5f;
        private Coroutine fadeOut;
        private GameObject fellaToRemove;
        public override void DoEvent()
        {
            if (!Object_Factory.TryGetObject(actorToRemove.name, out fellaToRemove)) return;
            Script_Manager.SM.activeActors.Remove(actorToRemove);
            List<Image> imgPo = new();
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
                Debug.LogWarning("Did not find actor: " + actorToRemove.name);
            }
        }

        private IEnumerator FadeOut(List<Image> imgPo)
        {
            float t = 0;

            while(t < fadeTime)
            {
                t += Time.deltaTime;
                Color tcol = Color.Lerp(Color.white, CoolColors.transparent, (t / fadeTime));
                foreach(Image ing in imgPo)
                {
                    ing.color = tcol;
                }
                yield return null;
            }
            Object_Factory.RemoveObject(actorToRemove.name);
        }

        private void PanicStop()
        {
            if(fadeOut != null) Script_Manager.SM.StopCoroutine(fadeOut);
            if (fellaToRemove != null) Object_Factory.RemoveObject(actorToRemove.name);
        }

        public override string ToString()
        {
            return "Actor/Remove Actor";
        }
    }
}
