using EXPERIMENTAL;
using UnityEngine;
using System;
using RenCSharp.Combat.Interfaces;
namespace RenCSharp.Combat.Player
{
    public class Natsuki_Spin : Player_Ability
    {
        [SerializeField, Tooltip("Needs a player_object and animated image handler")] private GameObject hammerFab;
        [SerializeField] private Sprite[] animationFrames;
        [SerializeField] private float secondsPerFrame = 0.1f;

        [Header("Audio")]
        [SerializeField] private AudioClip spawnShieldSFX;
        [SerializeField] private float minPitch;
        [SerializeField] private float maxPitch;

        private GameObject activeShield;

        public override void FireAbility()
        {
            if (!validToFire || PlayerTurn || activeShield != null) return;
            if(Object_Factory.TryGetObject("PlayerObject", out GameObject go))
            {
                t = 0;
                Event_Bus.TryFireFloatEvent("PlayerAbilityCooldown", t);
                Event_Bus.AddBoolEvent("EndAFight", GetRidOfShieldFR);
                int value = Flag_Manager.GetFlag(associatedTag);
                validToFire = false;
                activeShield = Object_Factory.SpawnObject(hammerFab, "NatsukiShield", go.transform); //will override previous with a new shield???
                Player_Object shield = activeShield.GetComponent<Player_Object>();
                IDamage playerIDamage = go.GetComponent<IDamage>();
                shield.ManualSetHealths(value, value);
                shield.CustomOnHitStuff += delegate { playerIDamage?.TakeDamage((object)0f, (object)false); };
                activeShield.GetComponent<Animated_Image_Handler>().ReceiveAnimationInformation(animationFrames, secondsPerFrame);
                playerIDamage.TakeDamage((object)0f, (object)false); //make player take 0 damage to give IFrames
                Audio_Manager.AM.Play2DSFX(spawnShieldSFX, minPitch, maxPitch);
            }
        }

        private void LateUpdate() //NASTY GROSS DISGUSTING WE DO NOT cARE!
        {
            if (activeShield == null) return;
            activeShield.transform.localPosition = Vector3.zero;
        }

        private void GetRidOfShieldFR(bool b)
        {
            Object_Factory.RemoveObject("NatsukiShield");
            if (Event_Bus.TryGetBoolEvent("EndAFight", out Action<bool> j))
            {
                j -= GetRidOfShieldFR;
            }
        }
    }
}
