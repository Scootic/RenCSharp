using UnityEngine;
namespace RenCSharp.Combat
{
    public static class TrigHelper
    {
        /// <summary>
        /// Do trig bullshit to get a quaternion out of a vec2. For bottom facing sprite type things. (projectiles).
        /// </summary>
        /// <param name="v2">Vector2 for relative dir pos</param>
        /// <returns>A quaternion with a euler angle z being relative and 0 x + 0 y.</returns>
        public static Quaternion GetQuaternion(Vector2 v2)
        {
            v2 /= v2.magnitude;
            float z = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg + 90;
            Vector3 euler = new Vector3(0, 0, z);
            return Quaternion.Euler(euler);
        }
        /// <summary>
        /// Do trig bullshit to get a quaternion out of a vec3. For bottom facing sprite type things. (projectiles).
        /// </summary>
        /// <param name="v3">Vector3 for relative dir pos</param>
        /// <returns>A quaternion with a euler angle z being relative and 0 x + 0 y.</returns>
        public static Quaternion GetQuaternion(Vector3 v3)
        {
            v3 /= v3.magnitude;
            float z = Mathf.Atan2(v3.y, v3.x) * Mathf.Rad2Deg + 90;
            Vector3 euler = new Vector3(0, 0, z);
            return Quaternion.Euler(euler);
        }
        /// <summary>
        /// Reflects a direction based on a normal
        /// </summary>
        /// <param name="ogDir">The original direction</param>
        /// <param name="normalDir">The direction of the normal</param>
        /// <returns>The reflected direction</returns>
        public static Vector3 ReflectDirection(Vector3 ogDir, Vector3 normalDir)
        {
            return ogDir - 2 * Vector3.Dot(ogDir, normalDir) * normalDir;
        }

        /// <summary>
        /// Get the position along a Bezier curve, based on an array of transforms.
        /// </summary>
        /// <param name="boundingPoints">The array of transforms that represent the bounding points</param>
        /// <param name="percent">The percent along the curve.</param>
        /// <returns>The vector3 that's percent% along the bezier curve.</returns>
        public static Vector3 BezPos(Transform[] boundingPoints, float percent)
        {
            Vector3 posToReturn = Vector3.zero; //default value because Vector3s can't be null for some stupid dumbass reason of doom

            //convert transforms to positions, because duh
            Vector3[] rawPositions = new Vector3[boundingPoints.Length];
            for (int i = 0; i < boundingPoints.Length; i++)
            {
                rawPositions[i] = boundingPoints[i].position;
            }

            //do initial step based on the rawPositions array
            Vector3[] recursiveLayer = new Vector3[rawPositions.Length - 1];
            for (int i = 0; i < recursiveLayer.Length; i++)
            {
                //store the lerped values between n1 and n2, n2 and n3, n3 and n4, etc. until finished.
                recursiveLayer[i] = Vector3.Lerp(rawPositions[i], rawPositions[i + 1], percent);
            }

            //once the initial recursiveLayer array has been setup, make a subarray to store the next step in bezier curve.
            //once subarray is finished, replace og recursiveLayer with the sublayer.
            //repeat until you are left with a single value.
            while (recursiveLayer.Length > 1)
            {
                Vector3[] subLayer = new Vector3[recursiveLayer.Length - 1];
                for (int i = 0; i < subLayer.Length; i++)
                {
                    subLayer[i] = Vector3.Lerp(recursiveLayer[i], recursiveLayer[i + 1], percent);
                }
                recursiveLayer = subLayer;
            }

            //return the single value found after (potentially) many lerps.
            posToReturn = recursiveLayer[0];
            return posToReturn;
        }

        /// <summary>
        /// Get the position along a Bezier curve, based on an array of world positions.
        /// </summary>
        /// <param name="boundingPoints">The array of world positions that represent the bounding points</param>
        /// <param name="percent">The percent along the curve.</param>
        /// <returns>The vector3 that's percent% along the bezier curve.</returns>
        public static Vector3 BezPos(Vector3[] boundingPoints, float percent)
        {
            Vector3 posToReturn = Vector3.zero; //default value because Vector3s can't be null for some stupid dumbass reason of doom

            //do initial step based on the boundingPoints array
            Vector3[] recursiveLayer = new Vector3[boundingPoints.Length - 1];
            for (int i = 0; i < recursiveLayer.Length; i++)
            {
                //store the lerped values between n1 and n2, n2 and n3, n3 and n4, etc. until finished.
                recursiveLayer[i] = Vector3.Lerp(boundingPoints[i], boundingPoints[i + 1], percent);
            }

            //once the initial recursiveLayer array has been setup, make a subarray to store the next step in bezier curve.
            //once subarray is finished, replace og recursiveLayer with the sublayer.
            //repeat until you are left with a single value.
            while (recursiveLayer.Length > 1)
            {
                Vector3[] subLayer = new Vector3[recursiveLayer.Length - 1];
                for (int i = 0; i < subLayer.Length; i++)
                {
                    subLayer[i] = Vector3.Lerp(recursiveLayer[i], recursiveLayer[i + 1], percent);
                }
                recursiveLayer = subLayer;
            }

            //return the single value found after (potentially) many lerps.
            posToReturn = recursiveLayer[0];
            return posToReturn;
        }
    }
}
