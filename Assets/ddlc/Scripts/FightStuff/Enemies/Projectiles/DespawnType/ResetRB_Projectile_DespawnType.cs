using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class ResetRB_Projectile_DespawnType : Projectile_DespawnType
    {
        [SerializeField] private Rigidbody rb;

        public override void OnDespawn(bool playerTurn, Transform despawningTransform)
        {
            rb.linearVelocity = Vector3.zero;
        }

        public override void OnEditorValidate()
        {
            
        }

        public override string ToString()
        {
            return "Reset RB Velocity";
        }
    }
}
