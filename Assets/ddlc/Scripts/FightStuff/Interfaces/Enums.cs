using UnityEngine;
namespace RenCSharp.Combat.Interfaces
{
    public enum AttackSpawnSelectionMethod
    {
        TrueRandom,
        NoRepeatRandom,
        LoopThrough,
        ReverseLoopThrough,
        PingPong
    }

    public enum BezierCurveType
    {
        SimpleArc,
        SCurve,
        BoomerangCurve
    }
    public struct BoundingBezierPositions
    {
        
        public static Vector3[] BoundingPositions4(BezierCurveType curveType, Vector3 startPosition, Vector3 fwdDirection, float arcDistance, float arcHeight)
        {
            Vector3[] arrayToReturn = new Vector3[4];
            Vector3 arcDir = Vector3.Cross(fwdDirection, Vector3.forward);

            arrayToReturn[0] = startPosition;

            switch (curveType)
            {
                case BezierCurveType.SimpleArc:
                    arrayToReturn[3] = arrayToReturn[0] + fwdDirection * arcDistance;
                    arrayToReturn[1] = arrayToReturn[0] + arcDir * arcHeight;
                    arrayToReturn[2] = arrayToReturn[3] + arcDir * arcHeight;
                    break;
                case BezierCurveType.SCurve:
                    arrayToReturn[1] = arrayToReturn[0] + arcDir * arcHeight;
                    arrayToReturn[2] = arrayToReturn[0] + fwdDirection * arcDistance;
                    arrayToReturn[3] = arrayToReturn[2] + arcDir * arcHeight;
                    break;
                case BezierCurveType.BoomerangCurve:
                    arrayToReturn[3] = arrayToReturn[0];
                    arrayToReturn[1] = arrayToReturn[0] + arcDir * arcHeight + fwdDirection * arcDistance;
                    arrayToReturn[2] = arrayToReturn[0] + fwdDirection * arcDistance * 2;
                    break;
            }

            return arrayToReturn;
        }
    }
}
