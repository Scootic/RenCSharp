using UnityEngine;
using RenCSharp.Actors;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
namespace RenCSharp.Sequences
{
    /// <summary>
    /// Spawns an actor into the scene using an Actor SO, an index that is passed to the Script_Manager as a place to spawn, and indexes for
    /// the appearance that the actor will take on.
    /// </summary>
    [Serializable]
    public class Spawn_Actor : Screen_Event
    {
        [SerializeField] private Actor actorToSpawn;
        [SerializeField, Tooltip("For coroutine to make the actor fade in. 0 for instantaneous spawning.")] private float fadeInTime = 0f;
        [SerializeField] private Vector3 spawnOffset = Vector3.zero;
        [SerializeField, Tooltip("Type in the light gray boxes.")] private string[] visualSpriteIndexes = new string[1];
        private Coroutine fadeIn;
        public override void DoShit()
        {
            //don't spawn another of an actor that already exists. save/load moment?
            if (Object_Factory.TryGetObject(actorToSpawn.ActorName, out GameObject go)) return;

            go = Object_Factory.SpawnObject(actorToSpawn.ActorPrefab, actorToSpawn.ActorName, Script_Manager.SM.ActorHolder);
            go.transform.position += spawnOffset;
            UI_Element uie = go.GetComponent<UI_Element>();

            for (int i = 0; i < visualSpriteIndexes.Length; i++) //loop through all sprites and assign thoroughly, only assign visuals to how many we have
            {
                if (visualSpriteIndexes[i] != string.Empty) uie.Images[i].sprite = actorToSpawn.Visuals[i].ReturnSprite(visualSpriteIndexes[i]);
                else uie.Images[i].sprite = actorToSpawn.Visuals[i].layer[0]; //grab default sprite if there's nothing assigned
            }
            fadeIn = Script_Manager.SM.StartCoroutine(FadeIn(uie));
            Script_Manager.ProgressScreenEvent += delegate { PanicStop(uie); };
        }

        private IEnumerator FadeIn(UI_Element uie)
        {
            float t = 0;
            Color transGender = new Color(1, 1, 1, 0);
            foreach (Image image in uie.Images) { image.color = transGender; }
            while(t <= fadeInTime)
            {
                t += Time.deltaTime;
                float perc = t / fadeInTime;
                Color tcol = Color.Lerp(transGender, Color.white, perc);

                foreach(Image image in uie.Images) { image.color = tcol; }
                yield return null;
            }
        }

        private void PanicStop(UI_Element uie)
        {
            Script_Manager.SM.StopCoroutine(fadeIn);
            foreach(Image image in uie.Images) { image.color = Color.white; }
        }

        public override string ToString()
        {
            return "Spawn Actor";
        }
    }
}
