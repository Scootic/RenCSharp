using UnityEngine;

namespace RenCSharp.Menus
{
    public abstract class Menu_Base : MonoBehaviour
    {
        public abstract void OnMenuOpen();
        public abstract void OnMenuClose();
    }
}
