using UnityEngine;

namespace RenCSharp
{
    public static class TransformExtend
    {
        public static void ReceiveToken(this Transform t, TransformToken tt)
        {
            t.SetLocalPositionAndRotation(tt.LocalPos, tt.LocalRot);
            t.localScale = tt.LocalScale;
        }
    }
}
