using System.Collections;
using UnityEngine;
using RenCSharp.Combat.Interfaces;
namespace RenCSharp.Combat.Enemies
{
    [RequireComponent(typeof(UI_Element))]
    public class Beam_Projectile : Base_Projectile
    {
        [Header("Beam")]
        [SerializeField, Tooltip("UI Element containing 4 Images: 0 - the actual proj image, 1 - the backdrop, " +
            "2 & 3 - fill up as collider progresses toward activation.")] private UI_Element beamVisualElements;
        [SerializeField] private AudioClip beamFullSound;
        [SerializeField, Range(0,1), Tooltip("Plays when collider is enabled.")] private float beamSoundVol = 1;
        [SerializeField, Tooltip("Applies to the child that has the visual element.")] private float beamVisualTravelSpeed = 0;
        [SerializeField] private Color emptyBeamC = Color.black;
        [SerializeField] private Color fullBeamC = Color.red;
        

        protected override IEnumerator EnableTriggerOverTime()
        {
            float t = 0;
            float eval;
            myCol.enabled = false;
            beamVisualElements.Images[0].enabled = false; //main image
            beamVisualElements.Images[0].transform.localPosition = Vector3.zero;
            beamVisualElements.Images[1].enabled = true; //underlay
            beamVisualElements.Images[2].enabled = true; //filler 1
            beamVisualElements.Images[3].enabled = true; //filler 2

            beamVisualElements.Images[2].fillAmount = 0;
            beamVisualElements.Images[3].fillAmount = 0;
            while (t <= colliderEnableTime)
            {
                t += Time.deltaTime;
                eval = t / colliderEnableTime;
                Color c = Color.Lerp(emptyBeamC, fullBeamC, eval);
                beamVisualElements.Images[2].fillAmount = eval;
                beamVisualElements.Images[2].color = c;
                beamVisualElements.Images[3].fillAmount = eval;
                beamVisualElements.Images[3].color = c;
                yield return null;
            }

            myCol.enabled = true;
            Audio_Manager.AM.Play2DSFX(beamFullSound, 0.9f, 1.01f, beamSoundVol, false);
            beamVisualElements.Images[0].enabled = true; //main image
            beamVisualElements.Images[1].enabled = false; //underlay
            beamVisualElements.Images[2].enabled = false; //filler 1
            beamVisualElements.Images[3].enabled = false; //filler 2
        }

        protected override void Update()
        {
            beamVisualElements.Images[0].transform.localPosition += Time.deltaTime * beamVisualTravelSpeed * Vector3.down;
            if ((movementWhileInactive && !myCol.enabled) || (movementWhileActive && myCol.enabled))
            {
                movementType.MovementBehavior();
            }
            if (updateTypes.Count <= 0) return;
            if ((customUpdateWhileInactive && !myCol.enabled) || (customUpdateWhileActive && myCol.enabled))
            {
                foreach (Projectile_CustomUpdate pcu in updateTypes)
                {
                    pcu.UpdateBehavior();
                }
            }
        }

        public override void OnRemove(bool playerTurn)
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
                    receiverRB = other.GetComponent<Rigidbody>();
                    switch (hitType)
                    {
                        case ProjectileHitType.StayStill:
                            actuallyTakeDamage = !Mathf.Approximately(receiverRB.linearVelocity.magnitude, 0f);
                            break;
                        case ProjectileHitType.StayMoving:
                            actuallyTakeDamage = Mathf.Approximately(receiverRB.linearVelocity.magnitude, 0f);
                            break;
                    }
                }
                if (actuallyTakeDamage) receiver.TakeDamage(baseDamage, false);
                if (onHitEffects.Count > 0)
                {
                    foreach (Projectile_OnHitEffect hitE in onHitEffects)
                    {
                        hitE.OnHit(other);
                    }
                }
            }
            if (destroyOnHit && receiver != null)
            {
                if (updateTypes.Count > 0)
                {
                    foreach (Projectile_CustomUpdate pcu in updateTypes)
                    {
                        pcu.OnRemove(false);
                    }
                }
                Object_Pooling.Despawn(gameObject);
            }
        }
    }
}
