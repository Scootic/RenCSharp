using EXPERIMENTAL;
using UnityEngine;
namespace RenCSharp.Combat
{
    public abstract class Player_Ability : MonoBehaviour
    {
        [Header("Ability Stats")]
        [SerializeField, Min(1)] protected float abilityCooldown = 2;
        [SerializeField, Tooltip("Handles whether or not an ability is unlocked by consulting the stupid " +
            "flag_manager about it. Obviously, a bitwise operation, so please use binary ah numbers.")] protected int requiredBit = 0;
        protected float t = 0;
        protected bool validToFire;

        public int RequiredBit => requiredBit;
        /// <summary>
        /// Always include .base before custom functionality, the base method handles timer logic.
        /// </summary>
        protected virtual void Update()
        {
            if (!Fight_Manager.FM.PlayerTurn) 
            { 
                if(t <= abilityCooldown) t += Time.deltaTime;
                if (t > abilityCooldown) t = abilityCooldown;
                float perc = t / abilityCooldown;
                Event_Bus.TryFireFloatEvent("PlayerAbilityCooldown", perc);
            }
        }
        /// <summary>
        /// Should fire whenever a fight starts, meaning ability cds are preserved through attacks, but not fights.
        /// </summary>
        protected void OnEnable()
        {
            t = 0;
        }

        /// <summary>
        /// Always include .base before custom functionality, the base method handles timer resetting
        /// </summary>
        public virtual void FireAbility()
        {
            if (!validToFire) return;
            t = 0;
            Event_Bus.TryFireFloatEvent("PlayerAbilityCooldown", t);
        }
    }
}
