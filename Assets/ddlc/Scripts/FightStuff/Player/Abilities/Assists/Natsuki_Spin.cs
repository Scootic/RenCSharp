using EXPERIMENTAL;
using System.Collections;
using UnityEngine;
using System;
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
            base.FireAbility();
            if (!validToFire || PlayerTurn || activeShield != null) return;
            if(Object_Factory.TryGetObject("PlayerObject", out GameObject go))
            {
                Event_Bus.AddBoolEvent("EndAFight", GetRidOfShieldFR);
                int value = Flag_Manager.GetFlag(associatedTag);
                validToFire = false;
                activeShield = Object_Factory.SpawnObject(hammerFab, "NatsukiShield", go.transform);
                activeShield.GetComponent<Player_Object>().ManualSetHealths(value, value);
                activeShield.GetComponent<Animated_Image_Handler>().ReceiveAnimationInformation(animationFrames, secondsPerFrame);
                Audio_Manager.AM.Play2DSFX(spawnShieldSFX, minPitch, maxPitch);
            }
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
