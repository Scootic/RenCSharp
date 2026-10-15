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
        /// Start the character collecting process and turn on the sine wobble coroutine.
        /// </summary>
        /// <param name="text">The TMPro Mesh. Actually need this guy fr.</param>
        /// <param name="speedValue">Frequency of sine wave.</param>
        /// <param name="yAmpValue">Optional y-amplitude multiplier (how big the sine wave jumps up/down)</param>
        protected static void SineText(TextMeshProUGUI text, string speedValue, string yAmpValue = "5")
        {
            if(float.TryParse(speedValue, out float speed))
            {
                Event_Bus.AddVoidEvent("LoadedSave", PanicStop);
                Event_Bus.AddVoidEvent("ProgressScreen", PanicStop);
                Event_Bus.AddSingleObjEvent("TextboxNewChar", AddCharToSineList);
                float yAmp = float.TryParse(yAmpValue, out yAmp) ? yAmp : 5f;
                routines.Add(TagRoutineHandler.TRH.StartCoroutine(SineTextRoutine(text, speed, yAmp)));
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

        private static IEnumerator SineTextRoutine(TextMeshProUGUI text, float speed, float yAmp)
        {
            Mesh mesh = text.mesh;
            //string ogSTR = text.text;
            Vector3 offset;
            Vector3[] vertices;
            Vector3[] ogVerts = new Vector3[0];
            while (text.text.Length >= 1) //since new texboxes make the text itself become empty, it should stop when ever screen progresses.
            {
                //text.ForceMeshUpdate();
                vertices = mesh.vertices;
                foreach(TMP_CharacterInfo c in affectedSineChars)
                {
                    if (ogVerts.Length == 0) ogVerts = vertices;

                    //if (vertices != null)
                    //{
                    //vertices = meshInfo[matInd].vertices
                    for(int j = c.vertexIndex; j < c.vertexIndex + 4; j++)
                    {
                        offset = SineWobble(Time.time * speed + (c.vertexIndex / 4) * Mathf.PI * 0.5f, yAmp); //offset guys by partial rotations of unit kirkle
                            //Debug.Log($"offset: {offset}, c.vertInd: {c.vertexIndex}");
                        vertices[j] = ogVerts[j] + offset;
                        ogVerts[j] = vertices[j] - offset;
                    }
                        //meshInfo[matInd].vertices = vertices;
                    //}
                    //int index = c.vertexIndex;
                    //for(int j = index; j < index + 4; j++)
                    //{
                    //   vertices[j] -= offset;
                    //}
                    //offset = SineWobble(Time.time * speed + i * Mathf.PI * 0.5f, yAmp); //offset guys by partial rotations of unit kirkle
                    //for(int j = index; j < index + 4; j++) //go through the four vertex corners of the character
                    //{
                    //    vertices[j] += offset;
                    //}
                }

                mesh.vertices = vertices;
                text.canvasRenderer.SetMesh(mesh);
                //text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                yield return null;
            }

            PanicStop();
            routines = new();
        }

        private static Vector3 SineWobble(float t, float yAmp = 5)
        {
            return new Vector3(Mathf.Cos(t), Mathf.Sin(t) * yAmp, 0);
        }
    }
}
