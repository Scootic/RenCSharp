using UnityEngine;
using System.Collections.Generic;
namespace EXPERIMENTAL
{
    public static class RandomHelper 
    {
        private static Dictionary<string, int> prevIRolls = new();

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

        public static void ClearPrevRolls()
        {
            prevIRolls.Clear();
        }
    }
}
