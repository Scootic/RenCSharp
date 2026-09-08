#if UNITY_EDITOR
using UnityEditor;
using RenCSharp.Editor;
namespace RenCSharp.Combat.Enemies.Editor
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
#endif