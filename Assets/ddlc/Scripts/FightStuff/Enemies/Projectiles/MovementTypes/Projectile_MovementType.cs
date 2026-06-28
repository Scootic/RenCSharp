using RenCSharp.EXPERIMENTAL;
using RenCSharp.Combat.Interfaces;
using System;
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    [Serializable]
    public abstract class Projectile_MovementType : IEditorValidate
    {
        /// <summary>
        /// Occurs in Update(), moves the projectile in a certain manner, determined by individual script.
        /// </summary>
        public abstract void MovementBehavior();

        public abstract void OnEditorValidate();

        public virtual void UpdateMoveDir(Vector3 v3, bool first = false)
        {
            moveDir = v3;
            if(movementSetsRotation || first) projectileTransform.rotation = TrigHelper.GetQuaternion(moveDir);
        }

        public Transform SetProjectileTransform { set { projectileTransform = value; } }
        public Rigidbody SetProjectileRigidbody { set { projectileRigidbody = value; } }
        [SerializeField] protected float speed = 500f;
        [SerializeField, Tooltip("Decides if the movement type affects rotation ever.")] protected bool movementSetsRotation = true;
        protected Transform projectileTransform;
        protected Rigidbody projectileRigidbody;
        protected Vector3 moveDir;

        public override string ToString()
        {
            return "Null MovementType";
        }
    }
}
