using UnityEngine;

namespace RenCSharp.Combat.Player
{
    public static class AbilityHelper
    {
        public static void AnimationTiming(int frameMidPoint, int animationSpriteLength, float animationDuration, out float middlePoint, out float remainder)
        {
            middlePoint = ((float)frameMidPoint / (float)animationSpriteLength) * animationDuration;
            remainder = animationDuration - middlePoint;
        }
    }
}
