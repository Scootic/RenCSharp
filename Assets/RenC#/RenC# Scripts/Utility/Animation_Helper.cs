using UnityEngine;

namespace RenCSharp
{
    public static class Animation_Helper 
    {
        private static AnimationCurve easeOut = new AnimationCurve(easeOutKeyframes);

        public static AnimationCurve EaseOut => easeOut;

        private static readonly Keyframe[] easeOutKeyframes =
        {
            new Keyframe(0, 0, 2, 2),
            new Keyframe(1,1,0,0)
        };
    }
}
