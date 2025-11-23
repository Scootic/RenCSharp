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
            List<Image> imagePonents = new();
            Transform placeWeWant = Script_Manager.SM.ActorHolder;
            GameObject spawnt = GameObject.Instantiate(actorToSpawn.ActorPrefab, placeWeWant);
            spawnt.transform.position += spawnOffset;
            spawnt.name = actorToSpawn.ActorName;
            Image image = spawnt.transform.GetChild(0).GetComponent<Image>();
            imagePonents.Add(image);
            for (int i = 0; i < visualSpriteIndexes.Length; i++) //loop through all sprites and assign thoroughly, only assign visuals to how many we have
            {
                if (visualSpriteIndexes[i] != string.Empty) image.sprite = actorToSpawn.Visuals[i].ReturnSprite(visualSpriteIndexes[i]);
                else image.sprite = actorToSpawn.Visuals[i].layer[0]; //grab default sprite if there's nothing assigned
                if (i < visualSpriteIndexes.Length - 1) { image = image.transform.GetChild(0).GetComponent<Image>(); imagePonents.Add(image); }
            }
            fadeIn = Script_Manager.SM.StartCoroutine(FadeIn(imagePonents));
            Script_Manager.ProgressScreenEvent += delegate { PanicStop(imagePonents); };
        }

        private IEnumerator FadeIn(List<Image> images)
        {
            float t = 0;
            Color transGender = new Color(0, 0, 0, 0);
            foreach (Image image in images) { image.color = transGender; }
            while(t <= fadeInTime)
            {
                t += Time.deltaTime;
                float perc = t / fadeInTime;
                Color tcol = Color.Lerp(transGender, Color.white, perc);

                foreach(Image image in images) { image.color = tcol; }
                yield return null;
            }
        }

        private void PanicStop(List<Image> images)
        {
            Script_Manager.SM.StopCoroutine(fadeIn);
            foreach(Image image in images) { image.color = Color.white; }
        }

        public override string ToString()
        {
            return "Spawn Actor";
        }
    }
}
