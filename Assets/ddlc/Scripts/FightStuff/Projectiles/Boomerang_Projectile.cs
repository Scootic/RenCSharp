using UnityEngine;

namespace RenCSharp.Combat
{
    public class Boomerang_Projectile : Base_Projectile
    {
        [Header("Boomeranging")]
        [SerializeField, Tooltip("How far the boomerang will go, along lifespan duration")] private float distanceFromSpawn;
        [SerializeField] private float arcHeight = 300f;
        [SerializeField] private AnimationCurve xCurve;
        [SerializeField] private AnimationCurve yCurve;

        private float eval;
        private float t;
        private Vector3 ogPos, endPos, midPos, arcDir;

        protected override void OnEnable()
        {
            base.OnEnable();
            t = 0;
        }

        public override void UpdateMoveDir(Vector3 v3)
        {
            moveDir = v3;
            ogPos = transform.position;
            endPos = ogPos + moveDir * distanceFromSpawn;
            midPos = Vector3.Lerp(ogPos, endPos, 0.5f);
            arcDir = Vector3.Cross(moveDir, Vector3.forward);
            midPos += arcDir * arcHeight;
            Debug.Log("Boomerang Positions: og" + ogPos + ",mid" +midPos + ",arcdir" + arcDir + ",end" + endPos);
        }

        protected override void Update()
        {
            t += Time.deltaTime;
            eval = t / lifetime;
            Vector3 newFramePos;
            Debug.Log("BoomerangEval: " + eval);
            if (eval <= 0.5f)
            {
                newFramePos = Vector3.Slerp(midPos, ogPos, 1 - xCurve.Evaluate(eval * 2));
            }
            else
            {
                newFramePos = Vector3.Slerp(endPos, midPos, 1 - xCurve.Evaluate(eval * 2 - 1));
            }

            Vector3 dirToFramePos = newFramePos - transform.position;
            if(dirToFramePos != Vector3.zero) transform.rotation = TrigHelper.GetQuaternion(dirToFramePos);

            transform.position = newFramePos;
        }
    }
}
