using UnityEngine;

namespace RenCSharp.Menus
{
    public class Menu_Manager : MonoBehaviour
    {
        public static Menu_Manager MM;
        private Menu_Base curMenu;
        [SerializeField] private Menu_Base[] allMenus;
        [SerializeField] private GameObject menusParent;

        private void Awake()
        {
            if (MM == null) MM = this;
            else if (MM != null) { Destroy(MM); MM = this; }
        }

        void Start()
        {
            curMenu = null;
        }

        public void OpenAMenu(int index)
        {
            menusParent.SetActive(true);
            if(Script_Manager.SM != null) Script_Manager.SM.PauseSequence();
            if (curMenu != null) curMenu.OnMenuClose();
            curMenu = allMenus[index];
            curMenu.OnMenuOpen();
        }

        public void CloseMenus()
        {
            if(Script_Manager.SM != null) Script_Manager.SM.UnpauseSequence();
            menusParent.SetActive(false);
        }
    }
}
