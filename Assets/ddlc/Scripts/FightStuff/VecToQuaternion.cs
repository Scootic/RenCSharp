using UnityEngine;
namespace RenCSharp.Combat
{
    public static class VecToQuaternion
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
        /// Do trig bullshit to get a quaternion out of a vec2. For bottom facing sprite type things. (projectiles).
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
    }
}
