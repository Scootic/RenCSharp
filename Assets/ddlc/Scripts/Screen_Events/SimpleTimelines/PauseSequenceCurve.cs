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

            if(EvaluateValue(time))Script_Manager.SM.PauseSequence(false);
            else Script_Manager.SM.UnpauseSequence();

        }

        public override bool EvaluateValue(float time)
        {
            TimelineKeyframe<bool> atTime = AtTime(time);
            return atTime.Value;
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Pause Sequence Curve not yet valid.";

            if (EvaluateValue(time)) return "Pausing Sequence";
            else return "Unpausing Sequence";
        }


        public override string ToString()
        {
            return "Pause Sequence Curve";
        }
    }
}
