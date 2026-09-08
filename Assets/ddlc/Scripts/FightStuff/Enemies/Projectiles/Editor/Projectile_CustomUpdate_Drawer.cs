#if UNITY_EDITOR
using UnityEditor;
using RenCSharp.Editor;
namespace RenCSharp.Combat.Enemies.Editor
{
    [CustomPropertyDrawer(typeof(Projectile_CustomUpdate))]
    public class Projectile_CustomUpdate_Drawer : PolymorphicPropertyDrawer<Projectile_CustomUpdate>
    {
        protected override string DropDownMenuName()
        {
            return "Select Custom Update Type";
        }
    }
}
#endif
