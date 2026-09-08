using UITK_SimpleTimeline;
namespace RenCSharp.Sequences
{
    public class ActorExpressionCurve : TypedTimelineCurve<VisualIndexes>, ILerpable
    {
        public override string ShorthandCurveName() => "Change Actor Expression Curve";
        public override string SpawnKeyframeName() => "Visual Indexes Keyframe";
        public override string ToAffectName() => "";
        private UI_Element uie = null;

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            try
            {
                TimelineKeyframe<VisualIndexes> toEval = AtTime(time);
                if (!Object_Factory.TryGetComponent(toEval.Value.ActorToSet.name, out uie)) return;

                for (int i = 0; i < toEval.Value.Length; i++)
                {
                    string s = toEval.Value.indexes[i];
                    if (s == string.Empty) continue;
                    uie.Images[i].sprite = toEval.Value.SpriteAtIndex(i,s);
                }
            }
            catch
            {
                return;
            }
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "";
            try
            {
                TimelineKeyframe<VisualIndexes> toEval = AtTime(time);
                string msg = "Should be setting Actor Expression to be: ";
                foreach(string s in toEval.Value.indexes)
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
