using System.Collections;
using UnityEngine;
using RenCSharp.Combat.Interfaces;
namespace RenCSharp.Combat.Enemies
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
        private bool activeHitbox;

        protected override IEnumerator EnableTriggerOverTime()
        {
            float t = 0;
            float eval;
            activeHitbox = false;
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
            activeHitbox = true;
            Audio_Manager.AM.Play2DSFX(beamFullSound, 0.9f, 1.01f, beamSoundVol, false);
            beamElements.Images[0].enabled = true; //main image
            beamElements.Images[1].enabled = false; //underlay
            beamElements.Images[2].enabled = false; //filler 1
            beamElements.Images[3].enabled = false; //filler 2
        }

        protected override void Update()
        {
            if(activeHitbox) transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        public override void OnDespawn(bool playerTurn)
        {
            StopCoroutine(spawnInRoutine);
        }

        protected override void OnTriggerEnter(Collider other)
        {
            receiver = other.GetComponent<IDamage>();
            bool actuallyTakeDamage = true;
            if (!damageOverTime && receiver != null)
            {
                if (hitType != ProjectileHitType.Normal)
                {
                    Rigidbody rb = other.GetComponent<Rigidbody>();
                    switch (hitType)
                    {
                        case ProjectileHitType.StayStill:
                            actuallyTakeDamage = rb.angularVelocity != Vector3.zero;
                            break;
                        case ProjectileHitType.StayMoving:
                            actuallyTakeDamage = rb.angularVelocity == Vector3.zero;
                            break;
                    }
                }
                if (actuallyTakeDamage) receiver.TakeDamage(baseDamage, false);
            }
            if (destroyOnHit && receiver != null)
            {
                Object_Pooling.Despawn(gameObject);
            }
        }
    }
}
