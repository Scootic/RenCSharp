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
        [SerializeField] private Sprite defaultImage;
        [Header("Main Menu Scene Loading")]
        [SerializeField] private Simple_Scene_Loader ssl;
        [SerializeField] private byte sceneToLoadIndex = 2;
        private int activeDatas = 0;
        private string fileName = "SaveData";
        public override void OnMenuOpen()
        {
            saveMenu.SetActive(true);
            SaveData[] allSDs = SaveLoad.FindAllSaves();

            for (int i = 0; i < allSDs.Length; i++)
            {
                int ind = i;
                UI_Element loadElement = Object_Factory.SpawnObject(loadGamePrefab, "Save"+ind, loadGameHolder).GetComponent<UI_Element>();

                loadElement.Texts[0].text = allSDs[ind].FileName != null ? allSDs[ind].FileName : "SaveData";
                if (allSDs[i].SaveScreenshot != null)
                {
                    Texture2D screenShotTexture = new Texture2D(2, 2);
                    screenShotTexture.LoadImage(allSDs[ind].SaveScreenshot);
                    Sprite spr = Sprite.Create(screenShotTexture, new Rect(0, 0, screenShotTexture.width, screenShotTexture.height), new Vector2(0.5f, 0.5f));
                    loadElement.Images[0].sprite = spr;
                }
                else
                {
                    loadElement.Images[0].sprite = defaultImage;
                }

                loadElement.Buttons[0].onClick.AddListener(delegate { Load(allSDs[ind]); });

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
            if (Script_Manager.SM != null)
            {
                Script_Manager.SM.LoadShit(sd);
                Menu_Manager.MM.CloseMenus(); //close after a save being loaded is probably the most sensible.
            }
            else
            {
                if(ssl == null)
                {
                    Debug.LogWarning("No Scene Loader Assigned to SaveLoad Menu");
                    return;
                }
                //we're on main menu doin' stuff
                SaveData_From_Main_Menu sdfmm = Object_Factory.SpawnObject(new GameObject(), "SL").AddComponent<SaveData_From_Main_Menu>();
                sdfmm.SD = sd;
                DontDestroyOnLoad(sdfmm.gameObject);
                ssl.LoadAnScene(sceneToLoadIndex);
            }
        }

        public void SetFileName(string s)
        {
            Regex.Replace(s, @"[^a-zA-Z0-9]+", ""); //get rid of any special characters
            if (s == string.Empty) s = "SaveData"; //if, for some bumfuck reason, you only have special characters, make it default back to SaveData;
            fileName = s;
        }

        public void Save()
        {
            Script_Manager.SM.SaveShit(fileName);
            OnMenuClose();
            OnMenuOpen();
        }
    }
}
