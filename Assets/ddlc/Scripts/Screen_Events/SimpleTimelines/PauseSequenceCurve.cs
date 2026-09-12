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
                TimelineKeyframe<bool> atTime = AtTime(time);
                if(atTime.Value)Script_Manager.SM.PauseSequence(false);
                else Script_Manager.SM.UnpauseSequence();
            }
            catch
            {
                return;
            }
        }

        public override string EvaluateMessage(float time)
        {
            if (!ValidCurve) return "Pause Sequence Curve not yet valid.";
            try
            {
                TimelineKeyframe<bool> atTime = AtTime(time);
                if (atTime.Value) return "Pausing Sequence";
                else return "Unpausing Sequence";
            }
            catch
            {
                return "Not flipping SM Pause.";
            }
        }


        public override string ToString()
        {
            return "Pause Sequence Curve";
        }
    }
}
