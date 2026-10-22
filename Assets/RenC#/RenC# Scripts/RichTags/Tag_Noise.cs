using RenCSharp.EXPERIMENTAL;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace RenCSharp.Tags
{
    public class Tag_Noise : Base_Tag
    {
        private static readonly List<List<TMP_CharacterInfo>> affectedNoiseChars = new();
        private static readonly List<Coroutine> routines = new();
        /// <summary>
        /// Fuggin' rando AF!!!!
        /// </summary>
        /// <param name="text">da text :)</param>
        /// <param name="maxDistance">FLOAT maximum distance from origin (x and y individually)!</param>
        /// <param name="deviation">FLOAT the deviation from rolled positions that are within christ's domain</param>
        protected static void NoiseText(TextMeshProUGUI text, string maxDistance, string deviation)
        {
            if (float.TryParse(maxDistance, out float maxDist) && float.TryParse(deviation, out float devi))
            {
                Event_Bus.AddVoidEvent("LoadedSave", PanicStop);
                Event_Bus.AddVoidEvent("ProgressScreen", PanicStop);
                Event_Bus.AddSingleObjEvent("TextboxNewChar", AddCharToNoiseList);
                affectedNoiseChars.Add(new());
                routines.Add(TagRoutineHandler.TRH.StartCoroutine(NoiseTextRoutine(text, maxDist, devi, affectedNoiseChars.Count - 1)));
            }
            else
            {
                Debug.LogWarning("NoiseText couldn't find a good max dist OR deviation!");
            }
        }

        protected static void EndNoiseText()
        {
            Event_Bus.TryRemoveSingleObjEvent("TextboxNewChar");
        }

        private static void PanicStop() 
        {
            foreach(Coroutine r in routines)
            {
                if(r != null) TagRoutineHandler.TRH.StopCoroutine(r);
            }
            EndNoiseText();
            if (Event_Bus.TryGetVoidEvent("LoadedSave", out Action fella))
            {
                fella -= PanicStop;
            }

            if(Event_Bus.TryGetVoidEvent("ProgressScreen", out Action goober))
            {
                goober -= PanicStop;
            }

            affectedNoiseChars.Clear();
        }

        private static void AddCharToNoiseList(object chara)
        {
            TMP_CharacterInfo c = (TMP_CharacterInfo)chara;
            if (affectedNoiseChars[^1].Contains(c) || c.character == ' ') return;
            //Debug.Log("Inputting char to list: " + c.character);
            affectedNoiseChars[^1].Add(c);
        }

        private static IEnumerator NoiseTextRoutine(TextMeshProUGUI text, float maxDist, float acceptableDeviation, int charList)
        {
            Vector3 offset;
            Vector3[] vertices;
            TMP_CharacterInfo c;
            yield return null; //wait a single frame to let character-truncating vertex adjustment happen first
            Mesh mesh = text.mesh;

            Vector3[] originalVertices = mesh.vertices;
            while (true) //go forever until stopped. ProgressScreen and LoadSave events called by SM will do it!
            {
                yield return null;
                vertices = mesh.vertices;
                for (int i = 0; i < affectedNoiseChars[charList].Count; i++)
                {
                    //if (originalVertices.Length == 0) originalVertices = vertices; 
                    c = affectedNoiseChars[charList][i];
                    offset = EvilNoise(maxDist, acceptableDeviation); //offset guys by partial rotations of unit circle
                    for (int j = c.vertexIndex; j < c.vertexIndex + 4; j++)
                    {
                        vertices[j] = originalVertices[j] + offset;
                        //originalVertices[j] = vertices[j] - offset;
                    }
                }

                mesh.vertices = vertices;
                text.canvasRenderer.SetMesh(mesh);
                //yield return null;
            }
        }
        private static Vector3 EvilNoise(float maximumer, float deviation = 0.25f)
        {
            return new Vector3(
                RandomHelper.NoRepeatRoll("xRandNoise", -maximumer, maximumer, deviation),
                RandomHelper.NoRepeatRoll("yRandNoise", -maximumer, maximumer, deviation),
                0);
        }
    }
}
