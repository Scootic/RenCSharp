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
    }
}
