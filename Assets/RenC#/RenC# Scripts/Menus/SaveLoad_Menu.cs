using UnityEngine;
using System.Text.RegularExpressions;
using RenCSharp.Sequences;
namespace RenCSharp.Menus
{
    public class SaveLoad_Menu : Menu_Base
    {
        [SerializeField] private GameObject saveMenu;
        [SerializeField] private GameObject loadGamePrefab;
        [SerializeField] private Transform loadGameHolder;
        private int activeDatas = 0;
        private string fileName = "SaveData";
        public override void OnMenuOpen()
        {
            saveMenu.SetActive(true);
            SaveData[] allSDs = SaveLoad.FindAllSaves();

            for (int i = 0; i < allSDs.Length; i++)
            {
                UI_Element loadElement = Object_Factory.SpawnObject(loadGamePrefab, "Save"+i, loadGameHolder).GetComponent<UI_Element>();

                loadElement.Texts[0].text = allSDs[i].FileName;

                Texture2D screenShotTexture = new Texture2D(2,2);
                screenShotTexture.LoadImage(allSDs[i].SaveScreenshot);
                Sprite spr = Sprite.Create(screenShotTexture, new Rect(0,0,screenShotTexture.width, screenShotTexture.height), new Vector2(0.5f,0.5f));
                loadElement.Images[0].sprite = spr;

                loadElement.Buttons[0].onClick.AddListener(delegate { Load(allSDs[i]); });

                activeDatas++;
            }
        }
        public override void OnMenuClose()
        {
            for (int i = activeDatas - 1; i >= 0; i--) 
            {
                Object_Factory.RemoveObject("Save" + i);
            }
            activeDatas = 0;
            saveMenu.SetActive(false);
        }

        private void Load(SaveData sd)
        {
            if (Script_Manager.SM != null) Script_Manager.SM.LoadShit(sd);
            else
            {
                //we're on main menu doin' stuff
            }
        }

        public void SetFileName(string s)
        {
            Regex.Replace(s, @"[^a-zA-Z0-9]+", ""); //get rid of any special characters
            if (s == string.Empty) s = "SaveData"; //if for some bumfuck reason, you only have special characters, make it default back to SaveData;
            fileName = s;
        }

        public void Save()
        {
            Script_Manager.SM.SaveShit(fileName);
        }
    }
}
