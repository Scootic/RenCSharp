using UnityEngine;

namespace RenCSharp
{
    public static class Animation_Helper 
    {
        private static AnimationCurve easeOut = new AnimationCurve(EaseOut1(), EaseOut2());

        public static AnimationCurve EaseOut => easeOut;

        private static Keyframe EaseOut1()
        {
            Keyframe toReturn = new Keyframe(0, 0, 2, 2, 0, 0);

            toReturn.tangentMode = 0;
            toReturn.weightedMode = WeightedMode.None;

            return toReturn;
        }
        private static Keyframe EaseOut2()
        {
            Keyframe toReturn = new Keyframe(1, 1, 0, 0, 0, 0);

            toReturn.tangentMode = 0;
            toReturn.weightedMode = WeightedMode.None;

            return toReturn;
        } 
    }
}
