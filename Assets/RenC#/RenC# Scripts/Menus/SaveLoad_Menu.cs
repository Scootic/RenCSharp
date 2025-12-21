using UnityEngine;

namespace RenCSharp.Menus
{
    public class SaveLoad_Menu : Menu_Base
    {
        [SerializeField] private GameObject saveMenu;
        [SerializeField] private GameObject loadGamePrefab;
        [SerializeField] private Transform loadGameHolder;
        public override void OnMenuOpen()
        {
            
        }
        public override void OnMenuClose()
        {
            
        }

        private void Load()
        {
            
        }

        public void Save(string fileName)
        {

        }
    }
}
