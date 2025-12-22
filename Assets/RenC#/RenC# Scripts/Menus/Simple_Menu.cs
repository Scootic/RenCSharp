using UnityEngine;

namespace RenCSharp.Menus
{
    public class Simple_Menu : Menu_Base
    {
        [SerializeField] private GameObject menuObject;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnMenuOpen()
        {
            menuObject.SetActive(true);
        }

        public override void OnMenuClose() 
        {
            menuObject.SetActive(false);
        }
    }
}
