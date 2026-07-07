using UnityEngine;
using System.Text.RegularExpressions;
using RenCSharp.Sequences;
namespace RenCSharp.Menus
{
    public class SaveLoad_Menu : Menu_Base
    {
        [SerializeField] private GameObject saveMenu;
        [SerializeField,Tooltip("Requires a UIE with 2 buttons, 1 image, and 1 TMPText")] private GameObject loadGamePrefab;
        [SerializeField, Tooltip("Preferably the content element of a scroll view.")] private Transform loadGameHolder;
        [SerializeField, Tooltip("Preferably the content element of a scroll view.")] private Transform autoSaveLoadGameHolder;
        [SerializeField, Tooltip("Displays if the save data has no screenshot information.")] private Sprite defaultImage;
        [SerializeField, Tooltip("Determines what saves are autosaves to sort them into a separate container.")] private string autoSavePattern = "!AutoSave_";
        [SerializeField, Tooltip("How many load game prefabs you expect to make up a row in the grid view.")] private int expectedPrefabsPerRow = 4;
        [Header("Main Menu Scene Loading")]
        [SerializeField] private bool mainMenu = false;
        [SerializeField] private Simple_Scene_Loader ssl;
        [SerializeField] private GameObject saveDataCarrierPrefab;
        [SerializeField] private byte sceneToLoadIndex = 2;
        private string subDirectory = "";
        private int activeDatas = 0;
        private string fileName = "SaveData";
        private Vector2 loadFabSizeDelta;
        private Awaitable openMenu;
        /// <summary>
        /// Rect Transform of Main Save Game scrolling content element
        /// </summary>
        private RectTransform rt;
        /// <summary>
        /// Rect Transform of Auto Save Game scrolling content element
        /// </summary>
        private RectTransform rt2;
        /// <summary>
        /// Rect Transform of GameObject saveMenu
        /// </summary>
        private RectTransform rt3;

        private void Start()
        {
            loadFabSizeDelta = loadGamePrefab.GetComponent<RectTransform>().sizeDelta;
            rt3 = saveMenu.GetComponent<RectTransform>();
        }

        public override async Awaitable OnMenuOpen()
        {
            saveMenu.SetActive(true);
            openMenu = MenuOpenVoid();
            await openMenu;
        }

        private async Awaitable MenuOpenVoid() //maybe eight frames per menu item?
        {
            for (int j = activeDatas - 1; j >= 0; j--) //say screw it, and get rid of all objects before hand.
            {
                Object_Factory.RemoveObject("Save" + j);
            }
            activeDatas = 0;
            await Awaitable.NextFrameAsync(); //give previous operation a frame of breathing room, lmao.

            if (!mainMenu)
            {
                try
                {
                    subDirectory = "Saves_" + Textbox_String.GetReplacerTexts["{mc}"];
                }
                catch
                {
                    subDirectory = "";
                }
            }
            else
            {
                subDirectory = "";
            }

            string[] paths = SaveLoad.AllSavesPaths(subDirectory);
            Debug.Log("Found save file paths length: " + paths.Length);

            rt = loadGameHolder.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(-0.5f * rt.sizeDelta.x - 50, 0);

            if(autoSaveLoadGameHolder != null)
            {
                rt2 = autoSaveLoadGameHolder.GetComponent<RectTransform>();
                rt2.anchoredPosition = new Vector2(-0.5f * rt2.sizeDelta.x - 50, 0);
            }
            
            for (int i = 0; i < paths.Length; i++)
            {
                if (!saveMenu.activeSelf) 
                { 
                    Debug.LogWarning("SaveMenu is no longer active, stopping the save grabbing process."); 
                    break; 
                } //just stop ts if we aren't even in the save menu no more
                SaveData sd = (SaveData)await SaveLoad.TryLoadFromPathAsync(paths[i]);
                bool auto = paths[i].Contains(autoSavePattern); //potential bug, doesn't care if it starts with autosave pattern, player can just do ts themselves
                await SpawnLoadButton(sd, i, auto);
                await Awaitable.NextFrameAsync();
            }
        }

        private async Awaitable SpawnLoadButton(SaveData sd, int i, bool auto)
        {
            GameObject go = await Object_Factory.SpawnObjectAsync(loadGamePrefab, "Save" + i, (auto && !mainMenu) ? autoSaveLoadGameHolder : loadGameHolder);
            UI_Element loadElement = go.GetComponent<UI_Element>();
            string s = sd.FileName;
            Regex.Replace(s, autoSavePattern, "");
            if (!mainMenu) 
            {
                loadElement.Texts[0].text = s;
            }
            else
            {
                try
                {
                    loadElement.Texts[0].text = sd.ReplacingTexts.Length > 0 ? $"{sd.ReplacingTexts[0]} - {s}" : s;
                }
                catch
                {
                    loadElement.Texts[0].text = s;
                }
            }
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
            loadElement.Buttons[1].onClick.AddListener(delegate { Delete(sd.FileName, i); });
            activeDatas++;

            float newSaveHolderSizeY = (loadGameHolder.transform.childCount / expectedPrefabsPerRow) * (loadFabSizeDelta.y * (rt3.sizeDelta.y / 257f));
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, newSaveHolderSizeY);
            if (autoSaveLoadGameHolder != null)
            {
                float newAutoSaveHolderSizeX = autoSaveLoadGameHolder.childCount * (loadFabSizeDelta.x) + loadFabSizeDelta.x;
                rt2.sizeDelta = new Vector2(newAutoSaveHolderSizeX, rt.sizeDelta.y);
            }
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
                if (ssl == null)
                {
                    Debug.LogWarning("No Scene Loader Assigned to SaveLoad Menu");
                    return;
                }
                Debug.Log("Loading from Menu");
                //we're on main menu doin' stuff
                SaveData_From_Main_Menu sdfmm = Object_Factory.SpawnObject(saveDataCarrierPrefab, "SL").GetComponent<SaveData_From_Main_Menu>();
                sdfmm.SD = sd;
                DontDestroyOnLoad(sdfmm.gameObject);
                ssl.LoadAnScene(sceneToLoadIndex);
            }
        }

        private void Delete(string saveFileName, int index)
        {
            SaveLoad.DeleteSaveFile(saveFileName);
            Object_Factory.RemoveObject("Save" + index);
        }

        public void SetFileName(string s)
        {
            Regex.Replace(s, @"[^a-zA-Z0-9]+", ""); //get rid of any special characters
            if (s == string.Empty) s = "SaveData"; //if, for some bumguck reason, you only have special characters, make it default back to SaveData;
            fileName = s;
        }

        public void Save()
        {
            Script_Manager.SM.SaveGameData(fileName);
            OnMenuClose();
            _ = OnMenuOpen();
        }
    }
}
