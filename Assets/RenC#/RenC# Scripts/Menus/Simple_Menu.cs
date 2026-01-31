using UnityEngine;

namespace RenCSharp.Menus
{
    public class Simple_Menu : Menu_Base
    {
        [SerializeField] private GameObject menuObject;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override async Awaitable OnMenuOpen()
        {
            await Awaitable.EndOfFrameAsync();
            menuObject.SetActive(true);
        }

        public override void OnMenuClose() 
        {
            menuObject.SetActive(false);
        }
    }
}
