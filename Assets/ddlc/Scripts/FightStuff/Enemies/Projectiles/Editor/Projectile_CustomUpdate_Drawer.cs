using UnityEditor;
namespace RenCSharp.Combat.Enemies
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
