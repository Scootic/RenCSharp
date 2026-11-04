using UnityEngine;
using UITK_SimpleTimeline;
namespace RenCSharp.Sequences
{
    
    public class OFRemoveObjectCurve : TypedTimelineCurve<string>
    {
        protected override int ExcludedKeyframeTangentModes() => 15; //exclude all but cliff
        public override string ShorthandCurveName() => "Remove Object Curve";
        public override string SpawnKeyframeName() => "String Keyframe";
        public override string ToAffectName() => "";

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            try
            {
                string s = AtTime(time).Value;
                Object_Factory.RemoveObject(s);
            }
            catch
            {
                return;
            }
        }

        public override string EvaluateValue(float time)
        {
            try
            {
                TimelineKeyframe<string> atTime = AtTime(time);
                return atTime.Value;
            }
            catch
            {
                return "";
            }
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Remove Object Curve not yet valid.";
            try
            {
                string b = AtTime(time).Value;
                return $"Removing Object: {b}";
            }
            catch
            {
                return "No Object to Remove!";
            }
        }


        public override string ToString()
        {
            return "Object Factory/Remove Object Curve";
        }
    }
    public class OFSpawnParticleCurve : TypedTimelineCurve<SpawnProjectile>
    {
        protected override int ExcludedKeyframeTangentModes() => 15; //exclude all but cliff
        public override string ShorthandCurveName() => "Spawn Particle Curve";
        public override string SpawnKeyframeName() => "Spawn Particle Keyframe";
        public override string ToAffectName() => "";

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            try
            {
                SpawnProjectile s = AtTime(time).Value;
                _ = Object_Factory.SpawnParticleObject(s);
            }
            catch
            {
                return;
            }
        }

        public override SpawnProjectile EvaluateValue(float time)
        {
            try
            {
                TimelineKeyframe<SpawnProjectile> atTime = AtTime(time);
                return atTime.Value;
            }
            catch
            {
                return new SpawnProjectile();
            }
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Spawn Projectile Curve not yet valid.";
            try
            {
                SpawnProjectile b = AtTime(time).Value;
                return $"Spawning Projectile: {b}";
            }
            catch
            {
                return "No Projectile to Spawn!";
            }
        }


        public override string ToString()
        {
            return "Object Factory/Spawn Particle Curve";
        }
    }

    /// <summary>
    /// object factory lol
    /// </summary>
    public class OFPositionCurve : TypedTimelineCurve<Vector3, string>, ILerpable
    {
        public override string ShorthandCurveName() => "OF Local Position Curve";
        public override string SpawnKeyframeName() => "Vector3 Keyframe";
        public override string ToAffectName() => "Object Factory Key to Move";
        private Transform t;
        public override void OnPlay()
        {
            Object_Factory.TryGetComponent(ToAffect, out t);
        }
        public override Vector3 EvaluateValue(float time)
        {
            if (!ValidCurve) return Vector3.zero;
            Vector3 toReturn;
            
            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = CurveMath.GetTangents(toEval);
            toReturn = CurveMath.CubicHermiteSpline(toEval[0].Value, toEval[1].Value,
                TimeToKeyframePercent(time, toEval[0].Time, toEval[1].Time), tangents[0], tangents[1], toEval[0].TangentMode);

            return toReturn;
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve || t == null) return;
            Vector3 toReturn = EvaluateValue(time);
            if (!toReturn.HasNaN()) t.localPosition = toReturn;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Local position curve not yet valid! Give it some keyframes!";
            return $"{ToAffect}.localPos should be: {EvaluateValue(time)}";
        }

        public override string ToString()
        {
            return "Object Factory/Set Local Position Curve";
        }
    }
    /// <summary>
    /// object factory lol
    /// </summary>
    public class OFJostlePositionCurve : TypedTimelineCurve<float, string>, ILerpable
    {
        public override string ShorthandCurveName() => "OF Jostle Position Curve";
        public override string SpawnKeyframeName() => "Float Keyframe";
        public override string ToAffectName() => "Object Factory Key to Jostle";
        private Vector3 ogLoc;
        private Transform t;
        public override void OnPlay()
        {
            Object_Factory.TryGetComponent(ToAffect, out t);
            ogLoc = t.localPosition;
        }

        public override float EvaluateValue(float time)
        {
            if (!ValidCurve) return 0;
            float rollDistance;
            
            TimelineKeyframe<float>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = CurveMath.GetTangents(toEval);
            rollDistance = CurveMath.CubicHermiteSpline(toEval[0].Value, toEval[1].Value,
                TimeToKeyframePercent(time, toEval[0].Time, toEval[1].Time), tangents[0], tangents[1], toEval[0].TangentMode);

            return rollDistance;
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve || t == null) return;
            t.localPosition = ogLoc + EvaluateValue(time) * CurveMath.RandomSineVector(-180f,180f);
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Jostle position curve not yet valid! Give it some keyframes!";
            return $"{ToAffect}.localPos should be within the distance range: {EvaluateValue(time)}";
        }

        public override string ToString()
        {
            return "Object Factory/Jostle Local Position Curve";
        }
    }
    /// <summary>
    /// object factory lol
    /// </summary>
    public class OFScaleCurve : TypedTimelineCurve<Vector3, string>, ILerpable
    {
        public override string ShorthandCurveName() => "OF Local Scale Curve";
        public override string SpawnKeyframeName() => "Vector3 Keyframe";
        public override string ToAffectName() => "Object Factory Key to Scale";
        private Transform t;
        public override void OnPlay()
        {
            Object_Factory.TryGetComponent(ToAffect, out t);
        }
        public override Vector3 EvaluateValue(float time)
        {
            if (!ValidCurve) return Vector3.zero;
            Vector3 toReturn;

            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = CurveMath.GetTangents(toEval);
            toReturn = CurveMath.CubicHermiteSpline(toEval[0].Value, toEval[1].Value,
                TimeToKeyframePercent(time, toEval[0].Time, toEval[1].Time), tangents[0], tangents[1], toEval[0].TangentMode);
            return toReturn;
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve || t == null) return;
            Vector3 toReturn = EvaluateValue(time);
            if (!toReturn.HasNaN()) t.localScale = toReturn;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Local scale curve not yet valid! Give it some keyframes!";

            return $"{ToAffect}.localScale should be: {EvaluateValue(time)}";
        }

        public override string ToString()
        {
            return "Object Factory/Set Local Scale Curve";
        }
    }
    /// <summary>
    /// object factory lol
    /// </summary>
    public class OFRotationCurve : TypedTimelineCurve<Vector3, string>, ILerpable
    {
        public override string ShorthandCurveName() => "OF Local Rotation Curve";
        public override string SpawnKeyframeName() => "Euler Angle (Vec3) Keyframe";
        public override string ToAffectName() => "Object Factory Key to Rotate";
        private Transform t;
        public override void OnPlay()
        {
            Object_Factory.TryGetComponent(ToAffect, out t);
        }

        public override Vector3 EvaluateValue(float time)
        {
            if (!ValidCurve) return Vector3.zero;
            TimelineKeyframe<Vector3>[] toEval = ClosestTwoKeyframes(time);

            float[] tangents = CurveMath.GetTangents(toEval);

            Vector3 eulerToBe = CurveMath.CubicHermiteSpline(toEval[0].Value, toEval[1].Value,
                TimeToKeyframePercent(time, toEval[0].Time, toEval[1].Time), tangents[0], tangents[1], toEval[0].TangentMode);
            return eulerToBe;
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve || t == null) return;
            Vector3 eulerToBe = EvaluateValue(time);
            t.localRotation = eulerToBe != Vector3.zero && !eulerToBe.HasNaN() ? Quaternion.Euler(eulerToBe) : Quaternion.identity;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Rotation curve is not yet valid! Give him some keyframes!";
            return $"{ToAffect}.localRot should be: {EvaluateValue(time)}.";
        }

        public override string ToString()
        {
            return "Object Factory/Set Local Rotation Curve";
        }
    }
}
