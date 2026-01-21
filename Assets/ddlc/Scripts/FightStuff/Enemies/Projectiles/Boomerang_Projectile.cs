using UnityEngine;
using EXPERIMENTAL;
using RenCSharp.Combat.Interfaces;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Jobs;
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
        private readonly Vector3[] boundingPositions = new Vector3[4];
        private NativeArray<Vector3> bounders;
        private TransformAccessArray me;
        private Vector3 arcDir;

        protected override void OnEnable()
        {
            bounders = new(boundingPositions, Allocator.Persistent);
            me = new(new Transform[] { transform },1);
            base.OnEnable();
            t = 0;
        }

        public override void UpdateMoveDir(Vector3 v3)
        {
            moveDir = v3;
            arcDir = Vector3.Cross(moveDir, Vector3.forward);

            switch (curveType) 
            {
                case BezierCurveType.SimpleArc:
                    boundingPositions[0] = transform.position;
                    boundingPositions[3] = boundingPositions[0] + moveDir * distanceFromSpawn;
                    boundingPositions[1] = boundingPositions[0] + arcDir * arcHeight;
                    boundingPositions[2] = boundingPositions[3] + arcDir * arcHeight;
                    break;
                case BezierCurveType.SCurve:
                    boundingPositions[0] = transform.position;
                    boundingPositions[1] = boundingPositions[0] + arcDir * arcHeight;
                    boundingPositions[2] = boundingPositions[0] + moveDir * distanceFromSpawn;
                    boundingPositions[3] = boundingPositions[2] + arcDir * arcHeight;
                    break;
            }
            //Debug.Log("Boomerang Positions: Array - " + boundingPositions[0] + "\n" + boundingPositions[1] + "\n"
            //    + boundingPositions[2] + "\n" + boundingPositions[3] + "\narcDir: " + arcDir); 
        }

        protected override void Update()
        {
            t += Time.deltaTime;
            eval = t / lifetime;
            Vector3 prevPos = transform.position;
            BezierPositionJob moveJob = new BezierPositionJob()
            {
                boundingPositions = bounders,
                percentAlongCurve = animateBezCurve.Evaluate(eval)
            };
            JobHandle handle = moveJob.Schedule(me);
            handle.Complete();

            Vector3 newFramePos = TrigHelper.BezPos(boundingPositions, animateBezCurve.Evaluate(eval));
            //Debug.Log("New Frame Pos: " + newFramePos + ", eval: " + eval);
            Vector3 dirToFramePos = transform.position - prevPos;
            if(dirToFramePos != Vector3.zero) transform.rotation = TrigHelper.GetQuaternion(dirToFramePos);
        }

        public override void OnDespawn(bool playerTurn)
        {
            base.OnDespawn(playerTurn);
            bounders.Dispose();
            me.Dispose();
        }
    }
}
