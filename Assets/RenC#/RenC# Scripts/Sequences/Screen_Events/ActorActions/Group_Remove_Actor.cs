using UnityEngine;
using RenCSharp.Actors;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using RenCSharp.EXPERIMENTAL;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// Removes an actor from the scene based on a passed in Actor SO.
    /// </summary>
    [Serializable]
    public class Group_Remove_Actor : Screen_Event
    {
        [SerializeField] private Actor[] actorsToRemove;
        [SerializeField, Tooltip("How long it takes for the actor to fade out."), Min(0f)] private float fadeTime = 0.5f;
        private Coroutine fadeOut;
        private List<GameObject> fellasToRemove;
        public override void DoEvent()
        {
            List<Image> imgPo = new();
            fellasToRemove = new();
            foreach (Actor actorToRemove in actorsToRemove)
            {
                if (!Object_Factory.TryGetObject(actorToRemove.name, out GameObject go)) return;
                Script_Manager.SM.activeActors.Remove(actorToRemove);
                fellasToRemove.Add(go);

                Image img = go.transform.GetChild(0).GetComponent<Image>();
                imgPo.Add(img);
                for (int i = 1; i < actorToRemove.Visuals.Length; i++)
                {
                    img = img.transform.GetChild(0).GetComponent<Image>();
                    imgPo.Add(img);
                }
            }

            fadeOut = Script_Manager.SM.StartCoroutine(FadeOut(imgPo));
            Script_Manager.ProgressScreenEvent += PanicStop;
        }

        private IEnumerator FadeOut(List<Image> imgPo)
        {
            float t = 0;

            while (t < fadeTime)
            {
                t += Time.deltaTime;
                Color tcol = Color.Lerp(Color.white, CoolColors.transparent, (t / fadeTime));
                foreach (Image ing in imgPo)
                {
                    ing.color = tcol;
                }
                yield return null;
            }
            foreach (Actor act in actorsToRemove)
            {
                Object_Factory.RemoveObject(act.name);
            }
        }

        private void PanicStop()
        {
            if (fadeOut != null) Script_Manager.SM.StopCoroutine(fadeOut);
            foreach (Actor act in actorsToRemove)
            {
                Object_Factory.RemoveObject(act.name);
            }
        }

        public override string ToString()
        {
            return "Actor/Remove Group of Actors";
        }
    }
}
