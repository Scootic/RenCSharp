using UnityEngine;
using System.Collections.Generic;
namespace EXPERIMENTAL
{
    public static class RandomHelper 
    {
        private static Dictionary<string, int> prevIRolls = new();
        private static Dictionary<string, float> prevFRolls = new();
        /// <summary>
        /// Rolls an integer that will be different than the previous int rolled under a string index.
        /// </summary>
        /// <param name="id">Which previous integer we shall check.</param>
        /// <param name="rangeLimit">Determines the range of the roll. It's exclusive.</param>
        /// <returns>The integer that's different from the previous one.</returns>
        public static int NoRepeatRoll(string id, int rangeLimit)
        {
            int roll = Random.Range(0, rangeLimit);
            if (prevIRolls.ContainsKey(id))
            {
                while(roll == prevIRolls[id])
                {
                    roll = Random.Range(0, rangeLimit);
                }
            }
            else
            {
                prevIRolls.Add(id, roll);
            }
            return roll;
        }
        /// <summary>
        /// Rolls a float that will be different than the previous float rolled under a string index.
        /// </summary>
        /// <param name="id">Which previous float we shall check.</param>
        /// <param name="minRoll">The minimum float value that can be rolled.</param>
        /// <param name="maxRoll">The maximum float value that can be rolled. Inclusive.</param>
        /// <param name="acceptableDeviation">The +- distance that the new roll must be outside compared to previous roll.</param>
        /// <returns></returns>
        public static float NoRepeatRoll(string id, float minRoll, float maxRoll, float acceptableDeviation)
        {
            float roll = Random.Range(minRoll, maxRoll);
            if (prevFRolls.ContainsKey(id))
            {
                while(roll < prevFRolls[id] + acceptableDeviation && roll > prevFRolls[id] - acceptableDeviation)
                {
                    roll = Random.Range(minRoll, maxRoll);
                }
            }
            else
            {
                prevFRolls.Add(id, roll);
            }
            return roll;
        }

        public static void ClearPrevRolls()
        {
            prevIRolls.Clear();
            prevFRolls.Clear();
        }
    }
}
