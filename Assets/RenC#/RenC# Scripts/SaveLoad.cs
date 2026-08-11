using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Text.RegularExpressions;
namespace RenCSharp
{
    /// <summary>
    /// This guy handles Save/Load. Go figure!
    /// </summary>
    public static class SaveLoad
    {
        private static readonly BinaryFormatter bf = new BinaryFormatter();
        /// <summary>
        /// Save a SaveData struct to a file in the persistent datapath.
        /// </summary>
        /// <param name="fileName">The name of the file (not file path), don't include the .fileType</param>
        /// <param name="sd">The savedata being saved to a file</param>
        /// <param name="subFolder"> The subfolder that data will be saved to. Useful for sorting save games into separate folders
        /// based on the player's name</param>
        public static void Save(string fileName, SaveData sd, string subFolder = "")
        {
            Regex.Replace(subFolder, "[ <>?*]", "_");
            string filePath = Application.persistentDataPath + "/" + (subFolder != "" ? subFolder + "/" : "") + fileName + ".sav";
            sd.FileName = fileName;
            Debug.Log("Saving data to: " + filePath);
            if(subFolder != "" && !Directory.Exists(Application.persistentDataPath + "/" + subFolder))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + subFolder);
            }
            FileStream fs = new FileStream(filePath, FileMode.Create);
            bf.Serialize(fs, sd);
            fs.Close();
        }
        /// <summary>
        /// Saves a flag token containing flags that apply regardless of save file. Use at your own discretion.
        /// </summary>
        /// <param name="ft">The flag token containing the flags you wish stored.</param>
        public static void SavePersistentFlags(FlagToken ft)
        {
            string filePath = Application.persistentDataPath + "/persistentFlags.fla";
            Debug.Log("Saving Persistent Flags!");
            FileStream fs = new FileStream(filePath, FileMode.Create);
            bf.Serialize(fs, ft);
            fs.Close();
        }
        /// <summary>
        /// Try to get SaveData information out of a file that bears the file name (not file path) you give.
        /// </summary>
        /// <param name="fileName">The name of the file that we're looking for.</param>
        /// <param name="sd">The SaveData we should be getting from that file.</param>
        /// <returns>True if that file exists, false if it does not.</returns>
        public static bool TryLoad(string fileName, out SaveData sd, string subFolder = "")
        {
            string filePath = Application.persistentDataPath + "/" + (subFolder != "" ? subFolder + "/" : "") + fileName + ".sav";
            sd = new SaveData();
            if (!File.Exists(filePath)) { Debug.LogWarning("No file at: " + filePath); return false; }

            FileStream fs = new FileStream(filePath, FileMode.Open);
            sd = (SaveData) bf.Deserialize(fs);
            Debug.Log("Found save data at: " + filePath);
            fs.Close();
            return true;
        }

        /// <summary>
        /// Uses awaitables to get some save data. ~20fps in the middle of loading thanks to offloading some stuffs to BG thread.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>The save data at filePath</returns>
        public static async Awaitable<SaveData?> TryLoadFromPathAsync(string filePath)
        {
            await Awaitable.BackgroundThreadAsync();
            SaveData? sd = null;
            if (!File.Exists(filePath) || !filePath.Contains(".sav"))
            {
                Debug.LogWarning("No/Bad file at: " + filePath);
                return null;
            }

            using (FileStream fs = File.Open(filePath, FileMode.Open))
            {
                sd = (SaveData?)bf.Deserialize(fs);
                //Debug.Log($"File exists? :{sd != null}");
            }

            await Awaitable.MainThreadAsync();
            return sd;
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

            FileStream fs = new FileStream(filePath, FileMode.Open);
            sd = (SaveData) bf.Deserialize(fs);
            fs.Close();
            if (sd == null) return false;
            return true;
        }

        /// <summary>
        /// Get the persistent flags from the persistentFlags file. Since they transcend specific save files, we always look for
        /// them at the same file location: Application.persistentDataPath/persistentFlags.fla
        /// </summary>
        /// <returns></returns>
        public static FlagToken LoadPersistentFlags()
        {
            FlagToken ft = new();
            if (!File.Exists(Application.persistentDataPath + "/persistentFlags.fla")) return ft;

            FileStream fs = new FileStream(Application.persistentDataPath + "/persistentFlags.fla", FileMode.Open);
            ft = (FlagToken)bf.Deserialize(fs);
            fs.Close();
            return ft;
        }
        /// <summary>
        /// Get all the savedatas from the Application.persistentDataPath. Used by the SaveLoad Menu to display all the saves
        /// a player could load or delete.
        /// </summary>
        /// <returns>An array of all found SaveData from the Application.persistentDataPath directory.</returns>
        public static SaveData[] FindAllSaves(string subDirectory = "")
        {
            List<SaveData> allSD = new();

            string[] allFilePaths;

            if (subDirectory == "") 
            { 
                allFilePaths = Directory.GetFiles(Application.persistentDataPath, "*.sav", SearchOption.AllDirectories);
            }
            else
            {
                allFilePaths = Directory.GetFiles(Application.persistentDataPath + "/" + subDirectory, "*.sav", SearchOption.AllDirectories);
            }

            foreach (string s in allFilePaths)
            {
                if (TryLoadFromPath(s, out SaveData? sd)) allSD.Add((SaveData)sd);
            }

            return allSD.ToArray();
        }

        public static int AllSavesLength() //?
        {
            string[] allFilePaths = Directory.GetFiles(Application.persistentDataPath, "*.sav", SearchOption.AllDirectories);
            return allFilePaths.Length;
        }

        public static string[] AllSavesPaths(string subDirectory = "")
        {
            string[] allFilePaths;
            if (subDirectory == "") 
            { 
                allFilePaths = Directory.GetFiles(Application.persistentDataPath, "*.sav", SearchOption.AllDirectories);
            }
            else 
            {
                Regex.Replace(subDirectory, "[ <>?*]", "_");
                allFilePaths = Directory.GetFiles(Application.persistentDataPath + "/" + subDirectory, "*.sav", SearchOption.AllDirectories);
            }
            return allFilePaths;
        }

        /// <summary>
        /// Specifically used to delete certain save data files, not any file at all contrary to the name.
        /// </summary>
        /// <param name="fileName">File name of save you want gone, not filepath!</param>
        /// <param name="subFolder">Subfolder namee :)</param>
        public static bool DeleteSaveFile(string fileName, string subFolder = "")
        {
            Regex.Replace(subFolder, "[ <>?*]", "_");
            string directoryPath = Application.persistentDataPath + "/" + (subFolder != "" ? subFolder + "/": "");
            string filePath = directoryPath + ".sav";
            if (!File.Exists(filePath)) { Debug.LogWarning("Trying to delete a save that doesn't exist at: " + filePath); return false; }
            Debug.Log($"Deleting file at: {filePath}");
            File.Delete(filePath);

            if(Directory.GetFiles(directoryPath, "*.sav", SearchOption.AllDirectories).Length == 0)
            {
                Debug.Log($"No more files at directory: {directoryPath}. Deleting directory.");
                Directory.Delete(directoryPath, true);
            }

            return true;
        }
    }
}
