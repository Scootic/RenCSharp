using UITK_SimpleTimeline;
namespace RenCSharp.Sequences
{
    public class PauseSequenceCurve : TypedTimelineCurve<bool>
    {
        protected override int ExcludedKeyframeTangentModes() => 15; //exclude all but cliff
        public override string ShorthandCurveName() => ToString();
        public override string SpawnKeyframeName() => "Bool Keyframe";
        public override string ToAffectName() => "";

        public override void Evaluate(float time)
        {
            if (!ValidCurve) return;
            try
            {
                bool b = AtTime(time).Value;
                if (b) Script_Manager.SM.PauseSequence(false);
                else Script_Manager.SM.UnpauseSequence();
            }
            catch
            {
                return;
            }
        }

        public override bool EvaluateValue(float time)
        {
            TimelineKeyframe<bool> atTime = AtTime(time);
            return atTime.Value;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Pause Sequence Curve not yet valid.";
            try
            {
                bool b = AtTime(time).Value;
                if (b) return "Pausing Sequence.";
                else return "Unpausing Sequence.";
            }
            catch
            {
                return "Neither pausing nor unpausing Sequence!";
            }
        }


        public override string ToString()
        {
            return "Pause Sequence Curve";
        }
    }
}
