#if UNITY_EDITOR
using UnityEditor;
using RenCSharp.Editor;
namespace RenCSharp.Combat.Enemies.Editor
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
#endif
