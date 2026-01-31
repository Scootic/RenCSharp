using EXPERIMENTAL;
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
        /// <param name="text"></param>
        /// <param name="maxDistance">!</param>
        /// <param name="deviation">FLOAT the deviation from rolled positions that are within christ's domain</param>
        protected static void NoiseText(TextMeshProUGUI text, string maxDistance, string deviation)
        {
            if (float.TryParse(maxDistance, out float maxDist) && float.TryParse(deviation, out float devi))
            {
                Event_Bus.AddVoidEvent("LoadedSave", PanicStop);
                Event_Bus.AddSingleObjEvent("TextboxNewChar", AddCharToNoiseList);
                routines.Add(TagRoutineHandler.TRH.StartCoroutine(NoiseTextRoutine(text, maxDist, devi)));
            }
            else
            {
                Debug.LogWarning("NoiseText couldn't find a good max dist or deviation!");
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

            affectedNoiseChars = new();
        }

        private static void AddCharToNoiseList(object chara)
        {
            TMP_CharacterInfo c = (TMP_CharacterInfo)chara;
            affectedNoiseChars.Add(c);
        }

        private static IEnumerator NoiseTextRoutine(TextMeshProUGUI text, float maxDist, float acceptableDeviation)
        {
            Mesh mesh = text.mesh;
            while (text.text.Length >= 1) //end the routine with a new screen, and reset the chars.
            {
                text.ForceMeshUpdate();
                Vector3[] verts = mesh.vertices;
                foreach(TMP_CharacterInfo c in affectedNoiseChars)
                {
                    int index = c.vertexIndex;
                    Vector3 offset = EvilNoise(maxDist, acceptableDeviation);
                    verts[index] += offset;
                    verts[index + 1] += offset;
                    verts[index + 2] += offset;
                    verts[index + 3] += offset;
                }
                mesh.vertices = verts;
                text.canvasRenderer.SetMesh(mesh);

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
