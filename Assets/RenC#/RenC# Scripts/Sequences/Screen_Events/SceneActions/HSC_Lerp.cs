using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace RenCSharp.Sequences
{
    public class HSC_Lerp : Screen_Event
    {
        [SerializeField, Tooltip("This will be the gameobject that the object factory tries to find whose material will be adjusted. " +
            "Requires an HSCShader Material. Only Background and Overlay hsc values can be saved for now.")] private string nameOfObject = "Background";
        [SerializeField, Tooltip("changes the given color of the image component.")] private Color newColor = Color.white;
        [SerializeField, Range(-360,360)] private float newHueDegree = 0;
        [SerializeField] private float newSaturation = 1;
        [SerializeField] private float newContrast = 1;
        [SerializeField, Tooltip("The time it takes for the hsc transition to complete.")] private float lerpDuration = 1;
        [SerializeField] private AnimationCurve lerpCurve = Animation_Helper.EaseOut;

        private Coroutine transitionRoutine;
        private float oldH, oldS, oldC, t, perc, lerpH, lerpS, lerpC;
        private Color oldColor;
        private Image image;
        private Material hscMat;
        public override void DoEvent()
        {
            if (Object_Factory.TryGetObject(nameOfObject, out GameObject go))
            {
                image = go.GetComponent<Image>();
                hscMat = new Material(image.material);
                image.material = hscMat;
                oldH = hscMat.GetFloat("_hue");
                oldS = hscMat.GetFloat("_sat");
                oldC = hscMat.GetFloat("_con");
                oldColor = image.color;
                t = 0;
                transitionRoutine = Script_Manager.SM.StartCoroutine(LerpHSC());
                Script_Manager.ProgressScreenEvent += PanicStop;
            }
            else
            {
                Debug.LogWarning($"couldn't find object of name: '{nameOfObject}' to hsc adjust.");
            }
        }

        private void PanicStop()
        {
            if (transitionRoutine != null) Script_Manager.SM.StopCoroutine(transitionRoutine);
            hscMat.SetFloat("_hue", newHueDegree);
            hscMat.SetFloat("_sat", newSaturation);
            hscMat.SetFloat("_con", newContrast);
            image.color = newColor;
            Script_Manager.ProgressScreenEvent -= PanicStop;
        }

        private IEnumerator LerpHSC()
        {
            while(t < lerpDuration)
            {
                t += Time.deltaTime;
                perc = lerpCurve.Evaluate(t / lerpDuration);

                lerpC = Mathf.Lerp(oldC, newContrast, perc);
                lerpH = Mathf.Lerp(oldH, newHueDegree, perc);
                lerpS = Mathf.Lerp(oldS, newSaturation, perc);

                hscMat.SetFloat("_hue", lerpH);
                hscMat.SetFloat("_sat", lerpS);
                hscMat.SetFloat("_con", lerpC);

                image.color = Color.Lerp(oldColor, newColor, perc);

                yield return null;
            }

            image.color = newColor;
            hscMat.SetFloat("_hue", newHueDegree);
            hscMat.SetFloat("_con", newContrast);
            hscMat.SetFloat("_sat", newSaturation);
        }

        public override string ToString()
        {
            return "Scene/HSC Lerp Adjustment";
        }
    }
}
