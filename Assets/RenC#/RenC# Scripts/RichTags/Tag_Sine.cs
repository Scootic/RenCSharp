using UnityEngine;
using TMPro;
using System.Collections;
using EXPERIMENTAL;
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
        /// <summary>
        /// Fuggin' rando AF!!!!
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxDistance">!</param>
        /// <param name="deviation">FLOAT the deviation from rolled positions that are within christ's domain</param>
        /// <param name="renderedText">guh</param>
        protected static void NoiseText(TextMeshProUGUI text, string maxDistance, string deviation, string renderedText)
        {
            if(float.TryParse(maxDistance, out float maxDist) && float.TryParse(deviation, out float devi))
            {
                int startchar = text.text.Length - 1;
                text.text = text.text.Insert(startchar, renderedText);
                int endchar = text.text.Length - 1;

                TagRoutineHandler.TRH.StartCoroutine(NoiseTextRoutine(text, startchar, endchar, maxDist, devi, renderedText));
            }
            else
            {
                Debug.LogWarning("NoiseText couldn't find a good max dist or deviation!");
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
                    Vector3 offset = SineWobble(Time.time * speed + (i - startchar) * Mathf.PI * 0.5f); //offset bastards by partial rotations of unit kirkle
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

        private static IEnumerator NoiseTextRoutine(TextMeshProUGUI text, int startchar, int endchar, float maxDist, float acceptableDeviation, string idtioRend)
        {
            Mesh mesh = text.mesh;
            while(text.text.Length >= endchar)
            {
                if (!text.text.Contains(idtioRend)) text.text = text.text.Insert(startchar, idtioRend);

                text.ForceMeshUpdate();
                Vector3[] verts = mesh.vertices;
                for(int i = startchar; i <= endchar; i++)
                {
                    TMP_CharacterInfo c = text.textInfo.characterInfo[i];
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
        }

        private static Vector3 SineWobble(float t)
        {
            return new Vector3(Mathf.Cos(t), Mathf.Sin(t) * 5, 0);
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
