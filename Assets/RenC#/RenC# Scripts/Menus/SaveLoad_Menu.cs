using UnityEngine;
using System.Text.RegularExpressions;
using RenCSharp.Sequences;
namespace RenCSharp.Menus
{
    public class SaveLoad_Menu : Menu_Base
    {
        [SerializeField] private GameObject saveMenu;
        [SerializeField,Tooltip("Requires a UIE with 2 buttons, 1 image, and 1 TMPText")] private GameObject loadGamePrefab;
        [SerializeField] private Transform loadGameHolder;
        [SerializeField] private Sprite defaultImage;
        [Header("Main Menu Scene Loading")]
        [SerializeField] private Simple_Scene_Loader ssl;
        [SerializeField] private byte sceneToLoadIndex = 2;
        private int activeDatas = 0;
        private string fileName = "SaveData";
        private Awaitable openMenu;
        public override async Awaitable OnMenuOpen()
        {
            saveMenu.SetActive(true);
            openMenu = MenuOpenVoid();
            await openMenu;
        }

        private async Awaitable MenuOpenVoid() //maybe eight frames?
        {
            int length = SaveLoad.AllSavesLength();
            string[] paths = SaveLoad.AllSavesPaths();
            RectTransform rt = loadGameHolder.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(-650, 0);
            for (int j = activeDatas - 1; j >= 0; j--) //say screw it, and get rid of all objects before hand.
            {
                Object_Factory.RemoveObject("Save" + j);
            }
            activeDatas = 0;
            await Awaitable.NextFrameAsync(); //give previous operation a frame of breathing room, lmao.
            for(int i = 0; i < length; i++)
            {
                if (!saveMenu.activeInHierarchy) break; //just stop ts if we aren't even in the save menu no more
                SaveData sd = (SaveData) await SaveLoad.TryLoadFromPathAsync(paths[i]);
                await SpawnLoadButton(sd, i);
                await Awaitable.NextFrameAsync();
            }
        }

        private async Awaitable SpawnLoadButton(SaveData sd, int i)
        {
            GameObject go = await Object_Factory.SpawnObjectAsync(loadGamePrefab, "Save" + i, loadGameHolder);
            UI_Element loadElement = go.GetComponent<UI_Element>();
            loadElement.Texts[0].text = sd.FileName != null ? sd.FileName : "SaveData";
            await Awaitable.NextFrameAsync();
            if (sd.SaveScreenshot != null)
            {
                Texture2D screenShotTexture = new Texture2D(2, 2);
                await Awaitable.NextFrameAsync();
                screenShotTexture.LoadImage(sd.SaveScreenshot);
                await Awaitable.NextFrameAsync();
                Sprite spr = Sprite.Create(screenShotTexture, new Rect(0, 0, screenShotTexture.width, screenShotTexture.height), new Vector2(0.5f, 0.5f));
                await Awaitable.NextFrameAsync();
                loadElement.Images[0].sprite = spr;
            }
            else
            {
                loadElement.Images[0].sprite = defaultImage;
            }

            loadElement.Buttons[0].onClick.AddListener(delegate { Load(sd); });
            loadElement.Buttons[1].onClick.AddListener(delegate { Delete(sd.FileName); });
            activeDatas++;
        }

        public override void OnMenuClose()
        {
            Debug.Log("Save menu closed!");
            saveMenu.SetActive(false);
        }

        private void Load(SaveData sd)
        {
            //if (!openMenu.IsCompleted) openMenu.Cancel();
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

        private async Awaitable Delete(string saveFileName)
        {
            SaveLoad.DeleteFile(saveFileName);
            OnMenuClose();
            await OnMenuOpen();
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
