using UnityEditor;

namespace RenCSharp.Combat.Enemies
{
    [CustomPropertyDrawer(typeof(Projectile_DespawnType))]
    public class Projectile_DespawnType_Drawer : PolymorphicPropertyDrawer<Projectile_DespawnType>
    {
        protected override string DropDownMenuName()
        {
            return "Select Despawn Behavior";
        }
    }
}
