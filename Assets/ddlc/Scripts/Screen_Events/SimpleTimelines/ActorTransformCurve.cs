using RenCSharp.Actors;
using UITK_SimpleTimeline;
using UnityEngine;
namespace RenCSharp.Sequences
{
    public class ActorLocalPositionArrayCurve : ArrayTimelineCurve<float, Actor>
    {
        public override int DefaultArrayLength() => 3;
        public override string[] SpawnKeyframeNames()
        {
            return new string[]
            {
                "X Keyframe",
                "Y Keyframe",
                "Z Keyframe"
            };
        }
        public override string SpawnKeyframeName() => "No?";
        public override string ToAffectName() => "Actor to Move";
        public override string ShorthandCurveName() => "Actor Relative Position Curve";

        public override void OnPlay()
        {
            if (Object_Factory.TryGetObject(ToAffect.name, out GameObject go))
            {
                SetRootObject(go);
                ogWorldPos = go.transform.position;
            }
            else
            {
                Debug.LogWarning($"Cannot animate Actor: {ToAffect.name}, because it's not present in the scene.");
            }
        }

        public override float[] EvaluateValue(float time)
        {
            TimelineKeyframe<float>[][] toEval = ArrayClosestTwoKeyframes(time);
            float[] toConvert = new float[3];
            for (int i = 0; i < toConvert.Length; i++)
            {
                float[] tangents = CurveMath.GetTangents(toEval[i]);
                toConvert[i] = CurveMath.CubicHermiteSpline(toEval[i][0].Value, toEval[i][1].Value,
                    TimeToKeyframePercent(time, toEval[i][0].Time, toEval[i][1].Time), tangents[0], tangents[1],
                    toEval[i][0].TangentMode);
            }
            return toConvert;
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            Vector3 eval = EvaluateValue(time).ToVector3();
            if (root && !eval.HasNaN()) root.transform.position = eval + ogWorldPos;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Actor Local Position Curve is not valid.";
            return $"Actor: {ToAffect.name}'s local position at {time}: {EvaluateValue(time).ToVector3()}";
        }

        private Vector3 ogWorldPos;

        public override string ToString()
        {
            return "Actor/Relative Position Array Curve";
        }
    }

    public class ActorLocalPositionCurve : TypedTimelineCurve<Vector3, Actor>
    {
        public override string ShorthandCurveName() => "Actor Relative Position Curve";
        public override string SpawnKeyframeName() => "Offset from Original Position (Vec3) Keyframe";
        public override string ToAffectName() => "Actor to Move";

        private Vector3 ogWorldPos;

        public override void OnPlay()
        {
            if (Object_Factory.TryGetObject(ToAffect.name, out GameObject go)) 
            {
                SetRootObject(go);
                ogWorldPos = go.transform.position;
            }
            else
            {
                Debug.LogWarning($"Cannot animate Actor: {ToAffect.name}, because it's not present in the scene.");
            }
        }

        public override Vector3 EvaluateValue(float time)
        {
            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = CurveMath.GetTangents(toEval);
            return CurveMath.CubicHermiteSpline(toEval[0].Value, toEval[1].Value,
                TimeToKeyframePercent(time, toEval[0].Time, toEval[1].Time), tangents[0], tangents[1], toEval[0].TangentMode);
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            Vector3 eval = EvaluateValue(time);
            if(root && !eval.HasNaN()) root.transform.position = eval + ogWorldPos;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Actor Local Position Curve is not valid.";
            return $"Actor: {ToAffect.name}'s local position at {time}: {EvaluateValue(time)}";
        }

        public override string ToString()
        {
            return "Actor/Relative Position Curve";
        }
    }
    public class ActorLocalScaleCurve : TypedTimelineCurve<Vector3, Actor>
    {
        public override string ShorthandCurveName() => "Actor Local Scale Curve";
        public override string SpawnKeyframeName() => "Vector3 Keyframe";
        public override string ToAffectName() => "Actor to Scale";

        public override void OnPlay()
        {
            if (Object_Factory.TryGetObject(ToAffect.name, out GameObject go)) 
            {
                SetRootObject(go);
            }
            else
            {
                Debug.LogWarning($"Cannot animate Actor: {ToAffect.name}, because it's not present in the scene.");
            }
        }

        public override Vector3 EvaluateValue(float time)
        {
            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = CurveMath.GetTangents(toEval);
            return CurveMath.CubicHermiteSpline(toEval[0].Value, toEval[1].Value,
                TimeToKeyframePercent(time, toEval[0].Time, toEval[1].Time), tangents[0], tangents[1], toEval[0].TangentMode);
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            Vector3 eval = EvaluateValue(time);
            if(root && !eval.HasNaN())root.transform.localScale = eval;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Actor Local Scale Curve is not valid.";
            return $"Actor: {ToAffect.name}'s local scale at {time}: {EvaluateValue(time)}";
        }

        public override string ToString()
        {
            return "Actor/Local Scale Curve";
        }
    }
    public class ActorLocalRotationCurve : TypedTimelineCurve<Vector3, Actor>
    {
        public override string ShorthandCurveName() => "Actor Local Rotation Curve";
        public override string SpawnKeyframeName() => "Euler Angle (Vec3) Keyframe";
        public override string ToAffectName() => "Actor to Rotate";

        public override void OnPlay()
        {
            if (Object_Factory.TryGetObject(ToAffect.name, out GameObject go)) 
            {
                SetRootObject(go);
            }
            else
            {
                Debug.LogWarning($"Cannot animate Actor: {ToAffect.name}, because it's not present in the scene.");
            }
        }

        public override Vector3 EvaluateValue(float time)
        {
            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = CurveMath.GetTangents(toEval);
            return CurveMath.CubicHermiteSpline(toEval[0].Value, toEval[1].Value,
                TimeToKeyframePercent(time, toEval[0].Time, toEval[1].Time), tangents[0], tangents[1], toEval[0].TangentMode);
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            Vector3 eval = EvaluateValue(time);
            if(root && !eval.HasNaN())root.transform.rotation = eval != Vector3.zero ? Quaternion.Euler(EvaluateValue(time)) : Quaternion.identity;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Actor Local Rotation Curve is not valid.";
            return $"Actor: {ToAffect.name}'s local rotation at {time}: {EvaluateValue(time)}";
        }

        public override string ToString()
        {
            return "Actor/Local Rotation Curve";
        }
    }
}
