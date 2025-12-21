using UnityEngine;

namespace RenCSharp.Menus
{
    public class Menu_Manager : MonoBehaviour
    {
        private Menu_Base curMenu;
        [SerializeField] private Menu_Base[] allMenus;
        [SerializeField] private GameObject menusParent;

        void Start()
        {
            curMenu = null;
        }

        public void OpenAMenu(int index)
        {
            menusParent.SetActive(true);
            Script_Manager.SM.PauseSequence();
            if (curMenu != null) curMenu.OnMenuClose();
            curMenu = allMenus[index];
            curMenu.OnMenuOpen();
        }

        public void CloseMenus()
        {
            Script_Manager.SM.UnpauseSequence();
            menusParent.SetActive(false);
        }
    }
}
