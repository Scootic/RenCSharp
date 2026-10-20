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
        //somehow only get these boys to the coroutine that needs 'em...
        private static readonly List<List<TMP_CharacterInfo>> affectedSineChars = new();
        private static readonly List<Coroutine> routines = new();
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
                affectedSineChars.Add(new());
                float yAmp = float.TryParse(yAmpValue, out yAmp) ? yAmp : 5f;
                routines.Add(TagRoutineHandler.TRH.StartCoroutine(SineTextRoutine(text, speed, yAmp, affectedSineChars.Count- 1)));
            }
            else
            {
                Debug.LogWarning("SineText couldn't find a stinkin' good speed value!");
            }
        }

        private static void AddCharToSineList(object chara) 
        { 
            TMP_CharacterInfo c = (TMP_CharacterInfo)chara;
            //Debug.Log("Adding char to sine list: " + c.character);
            if (affectedSineChars[^affectedSineChars.Count].Contains(c) || c.character == ' ') return;
            //char 0 = 0,1,2,3; char 1 = 4,5,6,7
            affectedSineChars[^affectedSineChars.Count].Add(c); //we only need to add this boy to the latest coroutine.
        }

        private static void PanicStop()
        {
            foreach(Coroutine r in routines)
            {
                if(r != null) TagRoutineHandler.TRH.StopCoroutine(r);
            }
            routines.Clear();
            EndSineText();
            if (Event_Bus.TryGetVoidEvent("LoadedSave", out Action fella))
            {
                fella -= PanicStop;
            }

            if(Event_Bus.TryGetVoidEvent("ProgressScreen", out Action goober))
            {
                goober -= PanicStop;
            }
            affectedSineChars.Clear();
        }
        //Called by TagParser: </SineText>
        protected static void EndSineText()
        {
            Event_Bus.TryRemoveSingleObjEvent("TextboxNewChar");
        }

        private static IEnumerator SineTextRoutine(TextMeshProUGUI text, float speed, float yAmp, int charList)
        {
            Vector3 offset;
            Vector3[] vertices;
            TMP_CharacterInfo c;
            int div4;
            yield return null; //wait a single frame to let character-truncating vertex adjustment happen first
            Mesh mesh = text.mesh;
            
            Vector3[] originalVertices = text.canvasRenderer.GetMesh().vertices;
            while (true) //go forever until stopped. ProgressScreen and LoadSave events called by SM will do it!
            {
                vertices = mesh.vertices;
                for (int i = 0; i < affectedSineChars[charList].Count; i++)
                {
                    //if (originalVertices.Length == 0) originalVertices = vertices; 
                    c = affectedSineChars[charList][i];
                    div4 = c.vertexIndex / 4; //used as part of the time offset for the sine wobble, essentially offset per character
                    offset = SineWobble(Time.time * speed + div4 * Mathf.PI * 0.5f, yAmp); //offset guys by partial rotations of unit circle
                    for (int j = c.vertexIndex; j < c.vertexIndex + 4; j++)
                    {
                        vertices[j] = originalVertices[j] + offset;
                        //originalVertices[j] = vertices[j] - offset;
                    }
                }

                mesh.vertices = vertices;
                text.canvasRenderer.SetMesh(mesh);
                yield return null;
            }
        }

        private static Vector3 SineWobble(float t, float yAmp = 5)
        {
            return new Vector3(Mathf.Cos(t), Mathf.Sin(t) * yAmp, 0);
        }
    }
}
