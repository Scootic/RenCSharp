using UnityEditor;

namespace RenCSharp.Combat.Enemies
{
    [CustomPropertyDrawer(typeof(Projectile_OnHitEffect))]
    public class Projectile_OnHitEffect_Drawer : PolymorphicPropertyDrawer<Projectile_OnHitEffect>
    {
        protected override string DropDownMenuName()
        {
            return "Select On Hit Effect";
        }
    }
}
