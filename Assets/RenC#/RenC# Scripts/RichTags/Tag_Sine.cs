using UnityEngine;
using TMPro;
using System.Collections;
using RenCSharp.EXPERIMENTAL;
using System.Collections.Generic;
using System;
namespace RenCSharp.Tags
{
    public class Tag_Sine : Base_Tag
    {
        private static List<TMP_CharacterInfo> affectedSineChars = new();
        private static List<Coroutine> routines = new();
        /// <summary>
        /// Sweet baby rays.
        /// </summary>
        /// <param name="text">The TMPro Mesh. Actually need this guy fr.</param>
        /// <param name="speedValue">Frequency of sine wave.</param>
        protected static void SineText(TextMeshProUGUI text, string speedValue)
        {
            if(float.TryParse(speedValue, out float speed))
            {
                Event_Bus.AddVoidEvent("LoadedSave", PanicStop);
                Event_Bus.AddVoidEvent("ProgressScreen", PanicStop);
                Event_Bus.AddSingleObjEvent("TextboxNewChar", AddCharToSineList);
                routines.Add(TagRoutineHandler.TRH.StartCoroutine(SineTextRoutine(text, speed)));
            }
            else
            {
                Debug.LogWarning("SineText couldn't find a stinkin' good speed value!");
            }
        }

        private static void AddCharToSineList(object chara) 
        { 
            TMP_CharacterInfo c = (TMP_CharacterInfo)chara;
            affectedSineChars.Add(c);
        }

        private static void PanicStop()
        {
            foreach(Coroutine r in routines)
            {
                if(r != null) TagRoutineHandler.TRH.StopCoroutine(r);
            }

            if (Event_Bus.TryGetVoidEvent("LoadedSave", out Action fella))
            {
                fella -= PanicStop;
            }

            if(Event_Bus.TryGetVoidEvent("ProgressScreen", out Action goober))
            {
                goober -= PanicStop;
            }

            affectedSineChars = new();
        }

        protected static void EndSineText()
        {
            Event_Bus.TryRemoveSingleObjEvent("TextboxNewChar");
        }

        private static IEnumerator SineTextRoutine(TextMeshProUGUI text, float speed)
        {
            Mesh mesh = text.mesh;
            string ogSTR = text.text;
            while (text.text.Length >= 1) //since new texboxes make the text itself become empty, it should stop when ever screen progresses.
            {
                text.ForceMeshUpdate();
                Vector3[] vertices = mesh.vertices;
                foreach(TMP_CharacterInfo c in affectedSineChars)
                {
                    int index = c.vertexIndex;
                    Vector3 offset = SineWobble(Time.time * speed + c.index * Mathf.PI * 0.5f); //offset guys by partial rotations of unit kirkle
                    vertices[index] += offset;
                    vertices[index + 1] += offset;
                    vertices[index + 2] += offset;
                    vertices[index + 3] += offset;
                }

                mesh.vertices = vertices;
                text.canvasRenderer.SetMesh(mesh);
                
                yield return null;
            }

            PanicStop();
            routines = new();
        }

        private static Vector3 SineWobble(float t)
        {
            return new Vector3(Mathf.Cos(t), Mathf.Sin(t) * 5, 0);
        }
    }
}
