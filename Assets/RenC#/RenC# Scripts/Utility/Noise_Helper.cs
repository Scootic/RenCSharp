using UnityEngine;

namespace RenCSharp
{
    public static class Noise_Helper
    {
        public const float TAU = Mathf.PI * 2;
        /// <summary>
        /// Get a Vector3 based on sine noise.
        /// </summary>
        /// <param name="minRad">The minimum radian angle</param>
        /// <param name="maxRad">The maximum radian angle</param>
        /// <returns>Sine values from three random radian angles</returns>
        public static Vector3 SineNoiseVector(float minRad, float maxRad)
        {
            Vector3 returner;

            float xRad = Random.Range(minRad, maxRad);
            float yRad = Random.Range(minRad, maxRad);
            float zRad = Random.Range(minRad, maxRad);

            returner = new Vector3(Mathf.Sin(xRad), Mathf.Sin(yRad), Mathf.Sin(zRad));

            return returner;
        }
    }
}
