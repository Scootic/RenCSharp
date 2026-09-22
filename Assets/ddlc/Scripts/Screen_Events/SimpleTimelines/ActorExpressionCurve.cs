using UITK_SimpleTimeline;
//using UnityEngine;
namespace RenCSharp.Sequences
{
    public class ActorExpressionCurve : TypedTimelineCurve<VisualIndexes>, ILerpable
    {
        protected override TimelineKeyframeTangentMode DefaultTangentMode() => TimelineKeyframeTangentMode.Cliff;
        protected override int ExcludedKeyframeTangentModes() => 15; //exclude all but cliff
        public override string ShorthandCurveName() => "Change Actor Expression Curve";
        public override string SpawnKeyframeName() => "Visual Indexes Keyframe";
        public override string ToAffectName() => "";


        private UI_Element uie = null;

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            try
            {
                VisualIndexes toEval = AtTime(time).Value;
                if (!Object_Factory.TryGetComponent(toEval.ActorToSet.name, out uie)) 
                {
                    //Debug.LogWarning($"Couldn't find {toEval.ActorToSet.name}");
                    return; 
                }

                for (int i = 0; i < toEval.Length; i++)
                {
                    string s = toEval.indexes[i];
                    if (s == string.Empty) continue;
                    uie.Images[i].sprite = toEval.SpriteAtIndex(i,s);
                }
            }
            catch
            {
                return;
            }
        }
        /// <summary>
        /// only exists for uitk preview
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public override VisualIndexes EvaluateValue(float time)
        {
            TimelineKeyframe<VisualIndexes>[] closest = ClosestTwoKeyframes(time);
            return closest[0].Value;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "";
            try
            {
                string msg = "Should be setting Actor Expression to be: ";
                foreach(string s in EvaluateValue(time).indexes)
                {
                    msg += "\n" + s;
                }
                return msg;
            }
            catch
            {
                return "Not changing Actor Expression!";
            }
        }

        public override string ToString()
        {
            return "Actor/Actor Expression Curve";
        }
    }
}
