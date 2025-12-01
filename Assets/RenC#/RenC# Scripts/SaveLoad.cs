using UnityEngine;

namespace RenCSharp
{
    public static class SaveLoad
    {
        public static void Save(string fileName)
        {

        }

        public static bool TryLoad(string fileName, out GameObject go)
        {
            go = null;
            return true;
        }
    }
}
