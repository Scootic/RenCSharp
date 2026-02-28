using UnityEngine;
using EXPERIMENTAL;
using RenCSharp.Combat.Interfaces;
namespace RenCSharp.Combat.Enemies
{
    public class Boomerang_Projectile : Base_Projectile
    {
        [Header("Boomeranging")]
        [SerializeField, Tooltip("How far the boomerang will go, along lifespan duration")] private float distanceFromSpawn;
        [SerializeField] private float arcHeight = 300f;
        [SerializeField] private AnimationCurve animateBezCurve;
        [SerializeField] private BezierCurveType curveType = BezierCurveType.SimpleArc;

        private float eval;
        private float t;
        private Vector3[] boundingPositions = new Vector3[4];

        protected override void OnEnable()
        {
            base.OnEnable();
            t = 0;
        }

        public override void UpdateMoveDir(Vector3 v3)
        {
            moveDir = v3;
            boundingPositions = BoundingBezierPositions.BoundingPositions4(curveType, transform.position, moveDir, distanceFromSpawn, arcHeight);

            //Debug.Log("Boomerang Positions: Array - " + boundingPositions[0] + "\n" + boundingPositions[1] + "\n"
            //    + boundingPositions[2] + "\n" + boundingPositions[3] + "\narcDir: " + arcDir); 
        }

        protected override void Update()
        {
            t += Time.deltaTime;
            eval = t / lifetime;
            Vector3 prevPos = transform.position;
            transform.position = TrigHelper.BezPos(boundingPositions, eval);

            //Debug.Log("New Frame Pos: " + newFramePos + ", eval: " + eval);
            Vector3 dirToFramePos = transform.position - prevPos;
            if(dirToFramePos != Vector3.zero) transform.rotation = TrigHelper.GetQuaternion(dirToFramePos);
        }

        public override void OnDespawn(bool playerTurn)
        {
            base.OnDespawn(playerTurn);
        }
    }
}
