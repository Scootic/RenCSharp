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
        private static List<TMP_CharacterInfo> affectedNoiseChars = new();
        private static List<Coroutine> routines = new();
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
                routines.Add(TagRoutineHandler.TRH.StartCoroutine(NoiseTextRoutine(text, maxDist, devi)));
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

            if (Event_Bus.TryGetVoidEvent("LoadedSave", out Action fella))
            {
                fella -= PanicStop;
            }

            if(Event_Bus.TryGetVoidEvent("ProgressScreen", out Action goober))
            {
                goober -= PanicStop;
            }

            affectedNoiseChars = new();
        }

        private static void AddCharToNoiseList(object chara)
        {
            TMP_CharacterInfo c = (TMP_CharacterInfo)chara;
            //Debug.Log("Inputting char to list: " + c.character);
            affectedNoiseChars.Add(c);
        }

        private static IEnumerator NoiseTextRoutine(TextMeshProUGUI text, float maxDist, float acceptableDeviation)
        {
            Mesh mesh = text.mesh;
            //string ogSTR = text.text;
            Vector3 offset;
            Vector3[] vertices;
            Vector3[] ogVerts = new Vector3[0];
            yield return null; //wait a single frame to let vertex adjustment happen first?

            while (text.text.Length >= 1) //since new texboxes make the text itself become empty, it should stop when ever screen progresses.
            {
                //text.ForceMeshUpdate();
                vertices = mesh.vertices;
                foreach (TMP_CharacterInfo c in affectedNoiseChars)
                {
                    if (ogVerts.Length == 0) ogVerts = vertices;

                    offset = EvilNoise(maxDist, acceptableDeviation);
                    for (int j = c.vertexIndex; j < c.vertexIndex + 4; j++)
                    { 
                        vertices[j] = ogVerts[j] + offset;
                        ogVerts[j] = vertices[j] - offset;
                    }
                }

                mesh.vertices = vertices;
                text.canvasRenderer.SetMesh(mesh);
                //text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                yield return null;
            }

            PanicStop();
            routines = new();
        }
        private static Vector3 EvilNoise(float maximumer, float deviation)
        {
            return new Vector3(
                RandomHelper.NoRepeatRoll("xRandNoise", -maximumer, maximumer, 0.25f),
                RandomHelper.NoRepeatRoll("yRandNoise", -maximumer, maximumer, 0.25f),
                0);
        }
    }
}
