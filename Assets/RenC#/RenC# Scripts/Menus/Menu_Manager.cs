using System;
using UnityEngine;

namespace RenCSharp.Menus
{
    public sealed class Menu_Manager : MonoBehaviour
    {
        public static Menu_Manager MM;
        private Menu_Base curMenu;
        [SerializeField] private MenusContainer[] menucontainers;

        private void Awake()
        {
            if (MM == null) MM = this;
            else if (MM != null) { Destroy(MM); MM = this; }
        }

        void Start()
        {
            curMenu = null;
        }
        /// <summary>
        /// Absolutely Bonkers BS. the tens space decides which menu container, the ones space decides which menu base to doo doo
        /// </summary>
        /// <param name="superIndex"></param>
        public void OpenAMenu(int superIndex)
        {
            int menuIndex = superIndex % 10;
            int containerIndex = (superIndex - menuIndex) / 10;
            menucontainers[containerIndex].MenuParent.SetActive(true);
            if(Script_Manager.SM != null) Script_Manager.SM.PauseSequence();
            if (curMenu != null) curMenu.OnMenuClose();
            curMenu = menucontainers[containerIndex].AllMenus[menuIndex];
            curMenu.OnMenuOpen();
        }

        public void CloseMenus()
        {
            if(Script_Manager.SM != null) Script_Manager.SM.UnpauseSequence();
            foreach(MenusContainer mc in menucontainers)
            {
                mc.MenuParent.SetActive(false);
            }
        }
        [Serializable]
        private struct MenusContainer 
        {
            public GameObject MenuParent;
            public Menu_Base[] AllMenus;
        }
    }
}
