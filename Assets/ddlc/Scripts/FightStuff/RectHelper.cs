using UnityEngine;

namespace RenCSharp.Combat
{
    public static class RectHelper
    {
        public static Rect Lerp(Rect r1, Rect r2, float t)
        {
            float newX = Mathf.Lerp(r1.x, r2.x, t);
            float newY = Mathf.Lerp(r1.y, r2.y, t);
            float newW = Mathf.Lerp(r1.width, r2.width, t);
            float newH = Mathf.Lerp(r1.height, r2.height, t);
            return new Rect(newX, newY, newW, newH);
        }
    }
}
