using System.Collections;
using UnityEngine;
using RenCSharp.Combat.Interfaces;
namespace RenCSharp.Combat
{
    [RequireComponent(typeof(UI_Element))]
    public class Beam_Projectile : Base_Projectile
    {
        [Header("Beam")]
        [SerializeField] private UI_Element beamElements;
        [SerializeField] private AudioClip beamFullSound;
        [SerializeField, Range(0,1)] private float beamSoundVol = 1;
        [SerializeField] private Color emptyBeamC = Color.black;
        [SerializeField] private Color fullBeamC = Color.red;

        protected override IEnumerator EnableTriggerOverTime()
        {
            float t = 0;
            float eval;
            myCol.enabled = false; 
            beamElements.Images[0].enabled = false; //main image
            beamElements.Images[1].enabled = true; //underlay
            beamElements.Images[2].enabled = true; //filler 1
            beamElements.Images[3].enabled = true; //filler 2

            beamElements.Images[2].fillAmount = 0;
            beamElements.Images[3].fillAmount = 0;
            while (t <= colliderEnableTime)
            {
                t += Time.deltaTime;
                eval = t / colliderEnableTime;
                Color c = Color.Lerp(emptyBeamC, fullBeamC, eval);
                beamElements.Images[2].fillAmount = eval;
                beamElements.Images[2].color = c;
                beamElements.Images[3].fillAmount = eval;
                beamElements.Images[3].color = c;
                yield return null;
            }

            myCol.enabled = true;
            Audio_Manager.AM.Play2DSFX(beamFullSound, 0.9f, 1.01f, beamSoundVol, false);
            beamElements.Images[0].enabled = true; //main image
            beamElements.Images[1].enabled = false; //underlay
            beamElements.Images[2].enabled = false; //filler 1
            beamElements.Images[3].enabled = false; //filler 2
        }

        protected override void Update()
        {
            //do jack shit, lmao!
        }

        protected override void OnTriggerEnter(Collider other)
        {
            receiver = other.GetComponent<IDamage>();
            if (!damageOverTime && receiver != null) receiver.TakeDamage(baseDamage, false);
            if (destroyOnHit && receiver != null)
            {
                Object_Pooling.Despawn(gameObject);
            }
        }
    }
}
