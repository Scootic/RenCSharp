using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
namespace RenCSharp
{
    public static class SaveLoad
    {
        public static void Save(string fileName, SaveData sd)
        {
            string filePath = Application.persistentDataPath + "/" + fileName + ".sav";
            sd.FileName = fileName;
            Debug.Log("Saving data to: " + filePath);
            FileStream fs = new FileStream(filePath, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, sd);
            fs.Close();
        }

        public static bool TryLoad(string fileName, out SaveData sd)
        {
            string filePath = Application.persistentDataPath + "/" + fileName + ".sav";
            sd = new SaveData();
            if (!File.Exists(filePath)) { Debug.LogWarning("No file at: " + filePath); return false; }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(filePath, FileMode.Open);
            sd = (SaveData) bf.Deserialize(fs);
            Debug.Log("Found save data at: " + filePath);
            fs.Close();
            return true;
        }
        /// <summary>
        /// Should make sure to only return files that we know for a fact are save datas. It should probably give null complaints
        /// in the event of finding non save files in the persistent datapath.
        /// </summary>
        /// <param name="filePath">The filepath the FileStream uses</param>
        /// <param name="sd">The Save Data we will try to return</param>
        /// <returns>True if there's a file at the filepath AND there's valid save data in that file.</returns>
        public static bool TryLoadFromPath(string filePath, out SaveData? sd)
        {
            if(!File.Exists(filePath) || !filePath.Contains(".sav")) { Debug.LogWarning("No/Bad file at: " + filePath); sd = null; return false; }
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(filePath, FileMode.Open);
            sd = (SaveData) bf.Deserialize(fs);
            fs.Close();
            if (sd == null) return false;
            return true;
        }

        public static SaveData[] FindAllSaves()
        {
            List<SaveData> allSD = new();

            string[] allFilePaths = Directory.GetFiles(Application.persistentDataPath);

            foreach(string s in allFilePaths)
            {
                if(TryLoadFromPath(s, out SaveData? sd)) allSD.Add((SaveData) sd);
            }

            return allSD.ToArray();
        }
    }
}
