using EXPERIMENTAL;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
public class TEMP_BezPosJob : TEMP_BezPosManual
{
    private NativeArray<Vector3> bound;
    private TransformAccessArray me;
    protected override void OnEnable()
    {
        base.OnEnable();
        me = new(new Transform[] { transform }, 1);
        bound = new NativeArray<Vector3>(realBounders, Allocator.Persistent);
        Debug.Log("bound length: " + bound.Length);
    }

    // Update is called once per frame
    override protected void Update()
    {
        t += Time.deltaTime;
        eval = t / timeToElapse;

        BezierPositionJob moveJob = new BezierPositionJob()
        {
            boundingPositions = bound,
            percentAlongCurve = eval,
        };
        JobHandle hanlde = moveJob.Schedule(me);
        hanlde.Complete();
        if (t >= timeToElapse) t = 0;
    }

    protected void OnDisable()
    {
        me.Dispose();
        bound.Dispose();
    }
}
