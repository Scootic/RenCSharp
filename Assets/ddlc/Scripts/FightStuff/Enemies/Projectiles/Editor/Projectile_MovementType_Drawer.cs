using UnityEditor;
namespace RenCSharp.Combat.Enemies
{
    [CustomPropertyDrawer(typeof(Projectile_MovementType))]
    public class Projectile_MovementType_Drawer : PolymorphicPropertyDrawer<Projectile_MovementType>
    {
        protected override string DropDownMenuName()
        {
            return "Select Projectile Movement Type";
        }
    }
}
