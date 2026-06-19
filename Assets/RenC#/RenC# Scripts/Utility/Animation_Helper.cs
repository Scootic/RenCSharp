using UnityEngine;

namespace RenCSharp
{
    public static class Animation_Helper 
    {
        public static readonly AnimationCurve EaseOut = new AnimationCurve(easeOutKeyframes);

        private static readonly Keyframe[] easeOutKeyframes =
        {
            new Keyframe(0, 0, 2, 2),
            new Keyframe(1,1,0,0)
        };
    }
}
