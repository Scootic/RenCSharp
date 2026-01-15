using UnityEngine;
using TMPro;
using System.Collections;
namespace RenCSharp.Tags
{
    public class Tag_Sine : Base_Tag
    {
        /// <summary>
        /// Sweet baby rays.
        /// </summary>
        /// <param name="text">The TMPro Mesh. Actually need this guy fr.</param>
        /// <param name="speedValue">Frequency of sine wave.</param>
        /// <param name="renderedText">The text we desire to wibble wobble.</param>
        protected static void SineText(TextMeshProUGUI text, string speedValue, string renderedText)
        {
            if(float.TryParse(speedValue, out float speed))
            {
                int startchar = text.text.Length - 1;
                text.text = text.text.Insert(startchar, renderedText); //crazee!
                int endchar = text.text.Length - 1;
                
                TagRoutineHandler.TRH.StartCoroutine(SineTextRoutine(text, speed, startchar, endchar, renderedText));
            }
            else
            {
                Debug.LogWarning("SineText couldn't find a stinkin' good speed value!");
            }
        }

        private static IEnumerator SineTextRoutine(TextMeshProUGUI text, float speed, int startchar, int endchar, string idiotrendertext)
        {
            Mesh mesh = text.mesh;

            while (text.text.Length >= endchar)
            {
                if (!text.text.Contains(idiotrendertext)) text.text = text.text.Insert(startchar, idiotrendertext); //hideous and bad!

                text.ForceMeshUpdate();
                Vector3[] vertices = mesh.vertices;
                for (int i = startchar; i <= endchar; i++)
                {
                    TMP_CharacterInfo c = text.textInfo.characterInfo[i];
                    int index = c.vertexIndex;
                    Vector3 offset = SineWobble(Time.time * speed + (i - startchar) * Mathf.PI * 0.5f);
                    vertices[index] += offset;
                    vertices[index + 1] += offset;
                    vertices[index + 2] += offset;
                    vertices[index + 3] += offset;
                }

                mesh.vertices = vertices;
                text.canvasRenderer.SetMesh(mesh);
                
                yield return null;
            }
        }

        private static Vector3 SineWobble(float t)
        {
            return new Vector3(Mathf.Cos(t), Mathf.Sin(t) * 5, 0);
        }
    }
}
