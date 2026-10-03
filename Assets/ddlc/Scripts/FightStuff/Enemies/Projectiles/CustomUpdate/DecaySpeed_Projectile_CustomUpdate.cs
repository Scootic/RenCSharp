using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    public class DecaySpeed_Projectile_CustomUpdate : Projectile_CustomUpdate
    {
        [SerializeField] private Base_Projectile me;
        [SerializeField] private AnimationCurve decayCurve = Animation_Helper.EaseDown;
        [SerializeField, Min(0)] private float decayTime = 3f;
        private float ogSpeed;
        private float t;
        private Projectile_MovementType pmt;
        public override void OnEnable()
        {
            pmt = me.GetMovementType;
            ogSpeed = pmt.GetSpeed;
            t = 0;
        }

        public override void UpdateBehavior()
        {
            if (t > decayTime) return;
            t += Time.deltaTime;
            pmt.SetSpeed = ogSpeed * decayCurve.Evaluate(t / decayTime);
        }

        public override void OnRemove(bool playerTurn)
        {
            pmt.SetSpeed = ogSpeed;
        }

        public override void OnEditorValidate()
        {
            return;
        }

        public override string ToString()
        {
            return "Decaying Speed";
        }
    }
}
