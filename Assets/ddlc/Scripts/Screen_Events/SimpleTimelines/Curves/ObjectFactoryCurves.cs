using UnityEngine;
using UITK_SimpleTimeline;
using UnityEngine.UI;
namespace RenCSharp.Sequences
{
    public class OFImageColorCurve : ArrayTimelineCurve<float,string>
    {
        public override string ShorthandCurveName() => "Color Image Curve";
        public override string SpawnKeyframeName() => "";
        public override string[] SpawnKeyframeNames() => new string[] {"R Keyframe", "G Keyframe", "B Keyframe", "Alpha Keyframe"};
        public override string ToAffectName() => "Object Factory Key to Fade";
        public override int DefaultArrayLength() => 4;
        private Image img;
        public override void OnPlay()
        {
            Object_Factory.TryGetComponent(ToAffect, out img);
        }
        public override float[] EvaluateValue(float time)
        {
            if (!ValidCurve) return new float[DefaultArrayLength()];
            float[] toReturn;
            try
            {
                toReturn = new float[] { img.color.r, img.color.b, img.color.g, img.color.a };
            }
            catch
            {
                toReturn = new float[4];
            }
            TimelineKeyframe<float>[][] toEval = ArrayClosestTwoKeyframes(time);
            for (int i = 0; i < toEval.Length; i++)
            {
                if (toEval[i] == null) continue;
                float[] tangents = CurveMath.GetTangents(toEval[i]);
                toReturn[i] = CurveMath.CubicHermiteSpline(toEval[i][0].Value, toEval[i][1].Value,
                    TimeToKeyframePercent(time, toEval[i][0].Time, toEval[i][1].Time), tangents[0], tangents[1], toEval[i][0].TangentMode);
            }

            return toReturn;
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve || img == null) return;
            img.color = EvaluateValue(time).ToColor();
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Color image curve not yet valid! Give it some keyframes!";
            if (img == null) return "No image to evaluate.";
            float[] eval = EvaluateValue(time);
            return $"{ToAffect}.color should be: R-{eval[0]}, G-{eval[1]}, B-{eval[2]}, A-{eval[3]}";
        }

        public override string ToString()
        {
            return "Object Factory/Color Image Curve";
        }
    }
    public class OFRemoveObjectCurve : TypedTimelineCurve<string>
    {
        protected override int ExcludedKeyframeTangentModes() => 15; //exclude all but cliff
        public override string ShorthandCurveName() => "Remove Object Curve";
        public override string SpawnKeyframeName() => "String Keyframe";
        public override string ToAffectName() => "";

        public override void Evaluate(float time)
        {
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
    public class OFPositionCurve : ArrayTimelineCurve<float, string>, ILerpable
    {
        public override string ShorthandCurveName() => "OF Local Position Curve";
        public override string[] SpawnKeyframeNames() => new string[]{"X Keyframe", "Y Keyframe", "Z Keyframe"};
        public override string SpawnKeyframeName()
        {
            return "";
        }
        public override string ToAffectName() => "Object Factory Key to Move";
        public override int DefaultArrayLength() => 3;
        private Transform t;
        public override void OnPlay()
        {
            Object_Factory.TryGetComponent(ToAffect, out t);
        }
        public override float[] EvaluateValue(float time)
        {
            if (!ValidCurve) return new float[] { 0,0,0};
            float[] toReturn = new float[3];
            
            TimelineKeyframe<float>[][] toEval = ArrayClosestTwoKeyframes(time);
            for (int i = 0; i < toReturn.Length; i++)
            {
                if (toEval[i] == null) continue;
                float[] tangents = CurveMath.GetTangents(toEval[i]);
                toReturn[i] = CurveMath.CubicHermiteSpline(toEval[i][0].Value, toEval[i][1].Value,
                    TimeToKeyframePercent(time, toEval[i][0].Time, toEval[i][1].Time), tangents[0], tangents[1], toEval[i][0].TangentMode);
            }
            return toReturn;
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve || t == null) return;
            Vector3 toReturn = EvaluateValue(time).ToVector3();
            if (!toReturn.HasNaN()) t.localPosition = toReturn;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Local position curve not yet valid! Give it some keyframes!";
            return $"{ToAffect}.localPos should be: {EvaluateValue(time).ToVector3()}";
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
    public class OFScaleCurve : ArrayTimelineCurve<float, string>, ILerpable
    {
        public override string ShorthandCurveName() => "OF Local Scale Curve";
        public override string[] SpawnKeyframeNames() => new string[] { "X Keyframe", "Y Keyframe", "Z Keyframe" };
        public override string SpawnKeyframeName()
        {
            return "";
        }
        public override int DefaultArrayLength() => 3;
        public override string ToAffectName() => "Object Factory Key to Scale";
        private Transform t;
        public override void OnPlay()
        {
            Object_Factory.TryGetComponent(ToAffect, out t);
        }
        public override float[] EvaluateValue(float time)
        {
            if (!ValidCurve) return new float[] {0,0,0};
            float[] toReturn = new float[3];

            TimelineKeyframe<float>[][] toEval = ArrayClosestTwoKeyframes(time);
            for (int i = 0; i < toEval.Length; i++)
            {
                if (toEval[i] == null) continue;
                float[] tangents = CurveMath.GetTangents(toEval[i]);
                toReturn[i] = CurveMath.CubicHermiteSpline(toEval[i][0].Value, toEval[i][1].Value,
                    TimeToKeyframePercent(time, toEval[i][0].Time, toEval[i][1].Time), tangents[0], tangents[1], toEval[i][0].TangentMode);
            }
            return toReturn;
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve || t == null) return;
            Vector3 toReturn = EvaluateValue(time).ToVector3();
            if (!toReturn.HasNaN()) t.localScale = toReturn;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Local scale curve not yet valid! Give it some keyframes!";

            return $"{ToAffect}.localScale should be: {EvaluateValue(time).ToVector3()}";
        }

        public override string ToString()
        {
            return "Object Factory/Set Local Scale Curve";
        }
    }
    /// <summary>
    /// object factory lol
    /// </summary>
    public class OFRotationCurve : ArrayTimelineCurve<float, string>, ILerpable
    {
        public override string ShorthandCurveName() => "OF Local Rotation Curve";
        public override string[] SpawnKeyframeNames() => new string[] { "X Keyframe", "Y Keyframe", "Z Keyframe" };
        public override string SpawnKeyframeName()
        {
            return "";
        }
        public override int DefaultArrayLength() => 3;
        public override string ToAffectName() => "Object Factory Key to Rotate";
        private Transform t;
        public override void OnPlay()
        {
            Object_Factory.TryGetComponent(ToAffect, out t);
        }

        public override float[] EvaluateValue(float time)
        {
            if (!ValidCurve) return new float[]{ 0,0,0};
            TimelineKeyframe<float>[][] toEval = ArrayClosestTwoKeyframes(time);
            float[] eulerToBe = new float[3];
            for (int i = 0; i < toEval.Length; i++)
            {
                if (toEval[i] == null) continue;
                float[] tangents = CurveMath.GetTangents(toEval[i]);

                eulerToBe[i] = CurveMath.CubicHermiteSpline(toEval[i][0].Value, toEval[i][1].Value,
                    TimeToKeyframePercent(time, toEval[i][0].Time, toEval[i][1].Time), tangents[0], tangents[1], toEval[i][0].TangentMode);
            }
            return eulerToBe;
        }

        public override void Evaluate(float time)
        {
            if (!ValidCurve || t == null) return;
            Vector3 eulerToBe = EvaluateValue(time).ToVector3();
            t.localRotation = eulerToBe != Vector3.zero && !eulerToBe.HasNaN() ? Quaternion.Euler(eulerToBe) : Quaternion.identity;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Rotation curve is not yet valid! Give him some keyframes!";
            return $"{ToAffect}.localRot should be: {EvaluateValue(time).ToVector3()}.";
        }

        public override string ToString()
        {
            return "Object Factory/Set Local Rotation Curve";
        }
    }
}
