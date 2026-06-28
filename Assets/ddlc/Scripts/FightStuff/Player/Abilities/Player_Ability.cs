using RenCSharp.EXPERIMENTAL;
using UnityEngine;
namespace RenCSharp.Combat.Player
{
    public abstract class Player_Ability : MonoBehaviour
    {
        [Header("Ability Stats")]
        [SerializeField, Min(1)] protected float abilityCooldown = 2;
        [SerializeField, Tooltip("Handles whether or not an ability is unlocked by consulting the stupid " +
        "flag_manager about it. Obviously, a bitwise operation, so please use binary ah numbers.")] protected int requiredBit = 0;
        [SerializeField, Tooltip("To be used by abilities that reference flags for certain values, like damage or resistances.")] protected string associatedTag;
        [SerializeField] protected bool firableOnSelect = false;
        [SerializeField] protected bool debug = false;
        protected float t;
        protected bool validToFire;

        [HideInInspector] public bool Current, PlayerTurn, Fighting;

        public float SetTimer { set { t = value; } }

        public bool FireableOnSelect => firableOnSelect;
        public int RequiredBit => requiredBit;
        /// <summary>
        /// Always include .base before custom functionality, the base method handles timer logic.
        /// </summary>
        protected virtual void Update()
        {
            if (!PlayerTurn && Fighting && Current) 
            {
                if(debug) Debug.Log("doing ability update: " + gameObject.name + ", t: " + t);
                t += Time.deltaTime;
                if (t >= abilityCooldown) { t = abilityCooldown; validToFire = true; }
                float perc = (float)t / (float)abilityCooldown;
                Event_Bus.TryFireFloatEvent("PlayerAbilityCooldown", perc);
            }
        }
        /// <summary>
        /// Should fire whenever a fight starts, meaning ability cds are preserved through attacks, but not fights.
        /// </summary>
        protected virtual void OnEnable()
        {
            t = 0;
        }

        /// <summary>
        /// Always include .base before custom functionality, the base method handles timer resetting
        /// </summary>
        public virtual void FireAbility()
        {
            if (!validToFire || PlayerTurn) return;
            t = 0;
            Event_Bus.TryFireFloatEvent("PlayerAbilityCooldown", t);
        }
    }
}
