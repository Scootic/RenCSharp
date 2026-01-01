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
        /// <param name="randomEveryAxis">Whether or not every axis rolls it's own value</param>
        /// <returns>Sine values from three random radian angles</returns>
        public static Vector3 SineNoiseVector(float minRad, float maxRad, bool randomEveryAxis = false)
        {
            Vector3 returner;

            float xRad = Random.Range(minRad, maxRad);

            if (randomEveryAxis)
            {
                float yRad = Random.Range(minRad, maxRad);
                float zRad = Random.Range(minRad, maxRad);

                returner = new Vector3(Mathf.Cos(xRad), Mathf.Sin(yRad), Mathf.Sin(zRad));
            }
            else
            {
                returner = new Vector3(Mathf.Cos(xRad), Mathf.Sin(xRad), Mathf.Sin(xRad));
            }
            
            Debug.Log("Rolled Sine Noise Direction: " + returner);

            return returner;
        }
    }
}
