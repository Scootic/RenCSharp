using UnityEngine;

namespace RenCSharp.Combat
{
    public class Boomerang_Projectile : Base_Projectile
    {
        [Header("Boomeranging")]
        [SerializeField, Tooltip("How far the boomerang will go, along lifespan duration")] private float distanceFromSpawn;
        [SerializeField] private float arcHeight = 300f;
        [SerializeField] private AnimationCurve animateBezCurve;

        private float eval;
        private float t;
        private Vector3[] boundingPositions = new Vector3[4];
        private Vector3 arcDir;

        protected override void OnEnable()
        {
            base.OnEnable();
            t = 0;
        }

        public override void UpdateMoveDir(Vector3 v3)
        {
            moveDir = v3;
            boundingPositions[0] = transform.position;
            boundingPositions[3] = boundingPositions[0] + moveDir * distanceFromSpawn;
            arcDir = Vector3.Cross(moveDir, Vector3.forward);
            boundingPositions[1] = boundingPositions[0] + arcDir * arcHeight;
            boundingPositions[2] = boundingPositions[3] + arcDir * arcHeight;
            Debug.Log("Boomerang Positions: Array - " + boundingPositions[0] + "\n" + boundingPositions[1] + "\n"
                + boundingPositions[2] + "\n" + boundingPositions[3] + "\narcDir: " + arcDir); 
        }

        protected override void Update()
        {
            t += Time.deltaTime;
            eval = t / lifetime;
            Vector3 newFramePos = TrigHelper.BezPos(boundingPositions, animateBezCurve.Evaluate(eval));
            Debug.Log("New Frame Pos: " + newFramePos + ", eval: " + eval);
            Vector3 dirToFramePos = newFramePos - transform.position;
            if(dirToFramePos != Vector3.zero) transform.rotation = TrigHelper.GetQuaternion(dirToFramePos);

            transform.position = newFramePos;
        }
    }
}
