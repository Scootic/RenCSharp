
using System.Collections.Generic;
using System;
namespace RenCSharp
{
    [Serializable]
    public struct FlagToken
    {
        public List<string> FlagIDs;
        public List<int> FlagValues;

        public FlagToken(Dictionary<string, int> dict)
        {
            FlagIDs = new List<string>();
            FlagValues = new List<int>();

            foreach(KeyValuePair<string,int> kvp in dict)
            {
                FlagIDs.Add(kvp.Key);
                FlagValues.Add(kvp.Value);
            }
        }
    }
}
