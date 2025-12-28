using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using EXPERIMENTAL;
using UnityEngine.UI;
namespace RenCSharp.Combat
{
    [RequireComponent(typeof(UI_Element))]
    public class EnemyObject : MonoBehaviour, IDamage
    {
        private UI_Element uie;
        private float curHealth;
        private List<Color> ogColors;
        [SerializeField, Range(0,1)] private float resistance = 0;
        [SerializeField, Min(0.01f)] private float damageAnimationTime = 0.5f, maxDistFromOG = 50f;
        [SerializeField] private AnimationCurve[] possibleCurves;
        private float t;
        private EnemySO meSo;
        public EnemySO MySO => meSo;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            uie = GetComponent<UI_Element>();
        }

        // Update is called once per frame
        IEnumerator TakeDamageVisual()
        {
            t = 0;
            Vector3 ogPos = transform.position;
            int xRoll = Random.Range(0, possibleCurves.Length);
            int yRoll = Random.Range(0, possibleCurves.Length);
            AnimationCurve x = possibleCurves[xRoll];
            AnimationCurve y = possibleCurves[yRoll];
            Vector3 rollPos = Noise_Helper.SineNoiseVector(0, Noise_Helper.TAU);
            float xDist = Random.Range(0, maxDistFromOG);
            float yDist = Random.Range(0, maxDistFromOG);
            Vector3 newPos = ogPos + new Vector3(rollPos.x * xDist, rollPos.y * yDist, 0);
            foreach (Image i in uie.Images)
            {
                i.color = Color.red;
            }
            while (t <= damageAnimationTime)
            {
                t += Time.deltaTime;
                float eval = t / damageAnimationTime;
                float midAnimX = Mathf.Lerp(ogPos.x, newPos.x, x.Evaluate(eval)); 
                float midAnimY = Mathf.Lerp(ogPos.y, newPos.y, y.Evaluate(eval)); 
                transform.position = new Vector3(midAnimX, midAnimY, 0);
                yield return null;
            }
            for (int i = 0; i < uie.Images.Length; i++)
            {
                uie.Images[i].color = ogColors[i];
            }
            transform.position = ogPos;
        }

        public void TakeDamage(float f, bool b)
        {
            float damageToTake = f - (f * resistance);
            curHealth -= damageToTake;
            if(curHealth <= 0)
            {
                //become killed af!
                //tell somebody about what happened.
                Fight_Manager.FM.EndAFight();
            }
            else
            {
                Event_Bus.TryFireFloatEvent("EnemyDamageNumber", damageToTake);
                StartCoroutine(TakeDamageVisual());
            }
        }

        public float Resistance()
        {
            return resistance;
        }

        public void ReceiveEnemySO(EnemySO so)
        {
            ogColors = new();
            meSo = so;
            for(int i = 0; i < so.VisualInformation.Length; i++)
            {
                uie.Images[i].sprite = so.VisualInformation[i];
                ogColors.Add(uie.Images[i].color);
            }
            curHealth = so.MaxHealth;
        }
    }
}
